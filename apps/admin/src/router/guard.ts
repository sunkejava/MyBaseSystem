import type { Router } from 'vue-router'
import { useUserStore } from '@/stores/user'
import { i18n } from '@/i18n'

const t = i18n.global.t

/**
 * 设置路由守卫
 * @param router - Vue Router 实例
 */
export function setupRouterGuard(router: Router) {
  router.beforeEach(async (to) => {
    const userStore = useUserStore()
    const token = localStorage.getItem('token')

    if (to.meta.requiresAuth !== false && !token) {
      return {
        name: 'Login',
        query: { redirect: to.fullPath },
      }
    }

    if (to.name === 'Login' && token) {
      return { name: 'Dashboard' }
    }

    if (to.meta.permissions?.length) {
      const userPermissions = userStore.permissions || []
      const hasPermission = to.meta.permissions.some((permission) =>
        userPermissions.includes(permission),
      )
      if (!hasPermission) {
        return { name: '403' }
      }
    }

    const titleKey = to.meta.titleKey as string
    const title = titleKey ? t(titleKey) : (to.meta.title as string)
    document.title = title ? `${title} | MyBaseSystem` : 'MyBaseSystem'
  })

  router.afterEach(() => {
    window.scrollTo(0, 0)
  })
}
