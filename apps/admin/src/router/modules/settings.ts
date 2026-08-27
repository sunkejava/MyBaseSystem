import type { RouteRecordRaw } from 'vue-router'
import {
  Settings,
  Palette,
  Bell,
  ShieldCheck,
  Settings2,
  Sun,
  Layout,
  List,
  UserLock,
} from 'lucide-vue-next'

/**
 * 系统设置路由模块
 * 包含三级路由结构
 */
const settingsRoutes: RouteRecordRaw[] = [
  {
    path: '/settings',
    name: 'Settings',
    redirect: '/settings/general',
    meta: {
      titleKey: 'menu.settings',
      title: 'Settings',
      icon: Settings,
      order: 4,
    },
    children: [
      {
        path: '/settings/general',
        name: 'SettingsGeneral',
        component: () => import('@/views/Settings.vue'),
        meta: {
          titleKey: 'menu.basicSettings',
          title: 'Basic Settings',
          icon: Settings2,
          order: 1,
          keepAlive: true,
        },
      },
      {
        path: '/settings/appearance',
        name: 'SettingsAppearance',
        redirect: '/settings/appearance/theme',
        meta: {
          titleKey: 'menu.appearance',
          title: 'Appearance',
          icon: Palette,
          order: 2,
        },
        children: [
          {
            path: '/settings/appearance/theme',
            name: 'SettingsTheme',
            component: () => import('@/views/settings/appearance/Theme.vue'),
            meta: {
              titleKey: 'menu.themeSettings',
              title: 'Theme Settings',
              icon: Sun,
              order: 1,
              keepAlive: true,
            },
          },
          {
            path: '/settings/appearance/layout',
            name: 'SettingsLayout',
            component: () => import('@/views/settings/appearance/Layout.vue'),
            meta: {
              titleKey: 'menu.layoutSettings',
              title: 'Layout Settings',
              icon: Layout,
              order: 2,
              keepAlive: true,
            },
          },
        ],
      },
      {
        path: '/settings/notification',
        name: 'SettingsNotification',
        redirect: '/settings/notification/list',
        meta: {
          titleKey: 'menu.notifications',
          title: 'Notifications',
          icon: Bell,
          order: 3,
        },
        children: [
          {
            path: '/settings/notification/list',
            name: 'NotificationList',
            component: () => import('@/views/settings/notification/NotificationList.vue'),
            meta: {
              titleKey: 'menu.notificationList',
              title: 'Notification List',
              icon: List,
              order: 1,
              keepAlive: true,
            },
          },
        ],
      },
      {
        path: '/settings/security',
        name: 'SettingsSecurity',
        redirect: '/settings/security/account',
        meta: {
          titleKey: 'menu.security',
          title: 'Security',
          icon: ShieldCheck,
          order: 4,
        },
        children: [
          {
            path: '/settings/security/account',
            name: 'SecurityAccount',
            component: () => import('@/views/settings/security/Account.vue'),
            meta: {
              titleKey: 'menu.accountSecurity',
              title: 'Account Security',
              icon: UserLock,
              order: 1,
              keepAlive: true,
            },
          },
        ],
      },
    ],
  },
]

export default settingsRoutes
