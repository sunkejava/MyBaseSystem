import type { RouteRecordRaw } from 'vue-router'
import {
  Users,
  Shield,
  Key,
  UserCog,
  List,
  UserCircle,
  ListChecks,
  ShieldCheck,
  LockKeyhole,
} from 'lucide-vue-next'

/**
 * 用户管理路由模块
 * 包含三级路由结构：
 * - 一级：/users (用户管理)
 * - 二级：/users/list (用户列表)
 * - 三级：/users/list/detail (用户详情)
 */
const usersRoutes: RouteRecordRaw[] = [
  {
    path: '/users',
    name: 'Users',
    redirect: '/users/list',
    meta: {
      titleKey: 'menu.users',
      title: 'Users',
      icon: Users,
      order: 2,
    },
    children: [
      {
        path: '/users/list',
        name: 'UsersList',
        redirect: '/users/list/index',
        meta: {
          titleKey: 'menu.userList',
          title: 'User List',
          icon: UserCog,
          order: 1,
        },
        children: [
          {
            path: '/users/list/index',
            name: 'UsersListIndex',
            component: () => import('@/views/Users.vue'),
            meta: {
              titleKey: 'menu.userListHome',
              title: 'User List Home',
              icon: List,
              order: 1,
              keepAlive: true,
            },
          },
          {
            path: '/users/list/detail/:id',
            name: 'UsersListDetail',
            component: () => import('@/views/users/detail/UserDetail.vue'),
            meta: {
              titleKey: 'menu.userDetail',
              title: 'User Detail',
              icon: UserCircle,
              order: 2,
              keepAlive: true,
              hideInMenu: true,
            },
          },
        ],
      },
      {
        path: '/users/roles',
        name: 'Roles',
        redirect: '/users/roles/list',
        meta: {
          titleKey: 'menu.roles',
          title: 'Roles',
          icon: Shield,
          order: 2,
        },
        children: [
          {
            path: '/users/roles/list',
            name: 'RolesList',
            component: () => import('@/views/users/Roles.vue'),
            meta: {
              titleKey: 'menu.roleList',
              title: 'Role List',
              icon: ListChecks,
              order: 1,
              keepAlive: true,
            },
          },
          {
            path: '/users/roles/detail/:id',
            name: 'RolesDetail',
            component: () => import('@/views/users/roles/RoleDetail.vue'),
            meta: {
              titleKey: 'menu.roleDetail',
              title: 'Role Detail',
              icon: ShieldCheck,
              order: 2,
              keepAlive: true,
              hideInMenu: true,
            },
          },
        ],
      },
      {
        path: '/users/permissions',
        name: 'Permissions',
        redirect: '/users/permissions/list',
        meta: {
          titleKey: 'menu.permissions',
          title: 'Permissions',
          icon: Key,
          order: 3,
        },
        children: [
          {
            path: '/users/permissions/list',
            name: 'PermissionsList',
            component: () => import('@/views/users/Permissions.vue'),
            meta: {
              titleKey: 'menu.permissionList',
              title: 'Permission List',
              icon: LockKeyhole,
              order: 1,
              keepAlive: true,
            },
          },
        ],
      },
    ],
  },
]

export default usersRoutes
