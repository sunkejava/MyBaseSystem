import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi, type TokenResponse, type UserProfile } from '@/api/client'

/**
 * 用户信息接口
 */
interface UserInfo {
  id: string
  name: string
  email: string
  avatar?: string
}

export const useUserStore = defineStore('user', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const storedUser = localStorage.getItem('user')
  const user = ref<UserInfo | null>(storedUser ? toUserInfo(JSON.parse(storedUser)) : null)
  const permissions = ref<string[]>(storedUser ? JSON.parse(storedUser).permissions || [] : [])

  const isAuthenticated = computed(() => token.value !== null)

  /**
   * 生成默认头像 URL（基于用户名）
   * 使用 DiceBear API 生成风格化头像
   */
  const defaultAvatar = computed(() => {
    if (user.value?.avatar) return user.value.avatar
    const name = user.value?.name || 'Admin'
    return `https://api.dicebear.com/7.x/avataaars/svg?seed=${encodeURIComponent(name)}&backgroundColor=b6e3f4,c0aede,d1d4f9,ffd5dc,ffdfbf`
  })

  function login(newToken: string, userData: UserInfo) {
    token.value = newToken
    user.value = userData
    localStorage.setItem('token', newToken)
  }

  function applySession(session: TokenResponse) {
    token.value = session.accessToken; user.value = toUserInfo(session.user); permissions.value = session.user.permissions
    localStorage.setItem('token', session.accessToken); localStorage.setItem('refreshToken', session.refreshToken); localStorage.setItem('user', JSON.stringify(session.user))
  }

  async function loginWithPassword(account: string, password: string, rememberMe: boolean) { const session = await authApi.login(account, password, rememberMe); applySession(session) }

  async function loadProfile() { const profile = await authApi.me(); user.value = toUserInfo(profile); permissions.value = profile.permissions; localStorage.setItem('user', JSON.stringify(profile)) }

  async function logout() {
    const refreshToken = localStorage.getItem('refreshToken'); if (refreshToken) { try { await authApi.logout(refreshToken) } catch {} }
    token.value = null
    user.value = null
    permissions.value = []
    localStorage.removeItem('token')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('user')
  }

  /**
   * 设置用户权限
   * @param perms - 权限列表
   */
  function setPermissions(perms: string[]) {
    permissions.value = perms
  }

  return {
    token,
    user,
    permissions,
    isAuthenticated,
    defaultAvatar,
    login,
    loginWithPassword,
    loadProfile,
    logout,
    setPermissions,
  }
})

function toUserInfo(profile: UserProfile): UserInfo { return { id: profile.id, name: profile.displayName || profile.userName, email: profile.email, avatar: profile.avatar } }
