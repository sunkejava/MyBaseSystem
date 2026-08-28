export interface ApiResult<T> { success: boolean; data: T; message: string; code?: string; traceId?: string }
export interface UserProfile { id: string; userName: string; displayName: string; email: string; avatar?: string; roles: string[]; permissions: string[] }
export interface TokenResponse { accessToken: string; refreshToken: string; expiresAt: string; user: UserProfile }
export interface PagedResult<T> { items: T[]; total: number; page: number; pageSize: number }
export interface UserListItem { id:string; userName:string; displayName:string; email:string; phone?:string; isEnabled:boolean; departmentId?:string; departmentName?:string; roleIds:string[]; roles:string[]; createdAt:string; lastLoginAt?:string }
export interface Role { id: string; code: string; name: string; description?: string; isSystem: boolean; isEnabled: boolean; permissions: string[] }
export interface Menu { id:string; parentId?:string; name:string; path:string; component?:string; icon?:string; permissionCode?:string; sort:number; type:string; hidden:boolean; isEnabled:boolean; children:Menu[] }
export interface Department { id:string; parentId?:string; code:string; name:string; sort:number; isEnabled:boolean; userCount:number; children:Department[] }
export interface Permission { id:string; code:string; name:string; group:string; description?:string }
export interface DashboardSummary { userCount:number; enabledUserCount:number; roleCount:number; departmentCount:number; todayLoginCount:number; todayRequestCount:number; unreadNotificationCount:number }
export interface NotificationItem { id:string; title:string; message:string; type:'info'|'success'|'warning'|'error'|'message'; isRead:boolean; actionUrl?:string; createdAt:string }
export interface AuditLog { id:string; userName:string; method:string; path:string; statusCode:number; elapsedMilliseconds:number; ipAddress?:string; createdAt:string }
export interface Setting { key:string; value:string; group:string; description?:string; isPublic:boolean }

const baseUrl = import.meta.env.VITE_API_BASE_URL || '/api/v1'
let refreshing: Promise<string | null> | null = null

async function renewToken(): Promise<string | null> {
  const refreshToken = localStorage.getItem('refreshToken')
  if (!refreshToken) return null
  const response = await fetch(`${baseUrl}/auth/refresh`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ refreshToken }) })
  if (!response.ok) return null
  const result: ApiResult<TokenResponse> = await response.json()
  localStorage.setItem('token', result.data.accessToken); localStorage.setItem('refreshToken', result.data.refreshToken); localStorage.setItem('user', JSON.stringify(result.data.user))
  return result.data.accessToken
}

export async function api<T>(path: string, init: RequestInit = {}, retry = true): Promise<T> {
  const headers = new Headers(init.headers); if (!(init.body instanceof FormData)) headers.set('Content-Type', 'application/json')
  const token = localStorage.getItem('token'); if (token) headers.set('Authorization', `Bearer ${token}`)
  let response = await fetch(`${baseUrl}${path}`, { ...init, headers })
  if (response.status === 401 && retry && !path.startsWith('/auth/')) {
    refreshing ||= renewToken().finally(() => { refreshing = null }); const fresh = await refreshing
    if (fresh) { headers.set('Authorization', `Bearer ${fresh}`); response = await fetch(`${baseUrl}${path}`, { ...init, headers }) }
  }
  if (!response.ok) { let message = `请求失败 (${response.status})`; try { const e = await response.json(); message = e.message || e.title || message } catch {} if (response.status === 401) { localStorage.removeItem('token'); localStorage.removeItem('refreshToken'); localStorage.removeItem('user') } throw new Error(message) }
  const result: ApiResult<T> = await response.json(); if (!result.success) throw new Error(result.message); return result.data
}

export const authApi = {
  login: (account: string, password: string, rememberMe: boolean) => api<TokenResponse>('/auth/login', { method: 'POST', body: JSON.stringify({ account, password, rememberMe }) }, false),
  logout: (refreshToken: string) => api<object>('/auth/logout', { method: 'POST', body: JSON.stringify({ refreshToken }) }),
  me: () => api<UserProfile>('/auth/me'),
}
export const systemApi = {
  users: (keyword = '', page = 1, pageSize = 20) => api<PagedResult<UserListItem>>(`/users?keyword=${encodeURIComponent(keyword)}&page=${page}&pageSize=${pageSize}`),
  createUser: (value: object) => api<string>('/users', { method: 'POST', body: JSON.stringify(value) }),
  updateUser: (id:string,value:object) => api<object>(`/users/${id}`, { method:'PUT',body:JSON.stringify(value) }),
  deleteUser: (id: string) => api<object>(`/users/${id}`, { method: 'DELETE' }),
  changePassword:(currentPassword:string,newPassword:string)=>api<object>('/users/me/password',{method:'PUT',body:JSON.stringify({currentPassword,newPassword})}),
  roles: () => api<Role[]>('/roles'),
  createRole: (value: object) => api<string>('/roles', { method: 'POST', body: JSON.stringify(value) }),
  updateRole: (id:string,value:object) => api<object>(`/roles/${id}`, { method:'PUT',body:JSON.stringify(value) }),
  deleteRole: (id: string) => api<object>(`/roles/${id}`, { method: 'DELETE' }),
  menus: () => api<Menu[]>('/menu-management'),
  createMenu: (value: object) => api<string>('/menu-management', { method: 'POST', body: JSON.stringify(value) }),
  updateMenu: (id:string,value:object) => api<object>(`/menu-management/${id}`, { method: 'PUT', body: JSON.stringify(value) }),
  deleteMenu: (id:string) => api<object>(`/menu-management/${id}`, { method: 'DELETE' }),
  departments: () => api<Department[]>('/departments'),
  createDepartment: (value:object) => api<string>('/departments', { method:'POST', body:JSON.stringify(value) }),
  updateDepartment: (id:string,value:object) => api<object>(`/departments/${id}`, { method:'PUT', body:JSON.stringify(value) }),
  deleteDepartment: (id:string) => api<object>(`/departments/${id}`, { method:'DELETE' }),
  permissions:()=>api<Permission[]>('/permissions'),
  summary:()=>api<DashboardSummary>('/dashboard/summary'),
  auditLogs:(page=1,pageSize=20)=>api<PagedResult<AuditLog>>(`/audit-logs?page=${page}&pageSize=${pageSize}`),
  notifications:()=>api<NotificationItem[]>('/notifications'),
  createNotification:(value:object)=>api<string>('/notifications',{method:'POST',body:JSON.stringify(value)}),
  markNotificationRead:(id:string)=>api<object>(`/notifications/${id}/read`,{method:'PUT'}),
  markAllNotificationsRead:()=>api<object>('/notifications/read-all',{method:'PUT'}),
  deleteNotification:(id:string)=>api<object>(`/notifications/${id}`,{method:'DELETE'}),
  settings:()=>api<Setting[]>('/settings'),
  saveSetting:(key:string,value:object)=>api<object>(`/settings/${encodeURIComponent(key)}`,{method:'PUT',body:JSON.stringify(value)}),
}
