import type { RouteRecordRaw } from 'vue-router'
import {
  BarChart3,
  LineChart,
  FileText,
  PieChart,
  FileBarChart,
  FileSpreadsheet,
  FileOutput,
} from 'lucide-vue-next'

/**
 * 数据分析路由模块
 * 包含三级路由结构
 */
const analyticsRoutes: RouteRecordRaw[] = [
  {
    path: '/analytics',
    name: 'Analytics',
    redirect: '/analytics/overview',
    meta: {
      titleKey: 'menu.analytics',
      title: 'Analytics',
      icon: BarChart3,
      order: 3,
    },
    children: [
      {
        path: '/analytics/overview',
        name: 'AnalyticsOverview',
        component: () => import('@/views/analytics/Overview.vue'),
        meta: {
          titleKey: 'menu.dataOverview',
          title: 'Data Overview',
          icon: PieChart,
          order: 1,
          keepAlive: true,
        },
      },
      {
        path: '/analytics/reports',
        name: 'Reports',
        redirect: '/analytics/reports/list',
        meta: {
          titleKey: 'menu.reports',
          title: 'Reports',
          icon: LineChart,
          order: 2,
        },
        children: [
          {
            path: '/analytics/reports/list',
            name: 'ReportsList',
            component: () => import('@/views/analytics/Reports.vue'),
            meta: {
              titleKey: 'menu.reportList',
              title: 'Report List',
              icon: FileBarChart,
              order: 1,
              keepAlive: true,
            },
          },
          {
            path: '/analytics/reports/detail/:id',
            name: 'ReportsDetail',
            component: () => import('@/views/analytics/reports/ReportDetail.vue'),
            meta: {
              titleKey: 'menu.reportDetail',
              title: 'Report Detail',
              icon: FileSpreadsheet,
              order: 2,
              keepAlive: true,
              hideInMenu: true,
            },
          },
        ],
      },
      {
        path: '/analytics/export',
        name: 'AnalyticsExport',
        redirect: '/analytics/export/list',
        meta: {
          titleKey: 'menu.export',
          title: 'Export',
          icon: FileText,
          order: 3,
        },
        children: [
          {
            path: '/analytics/export/list',
            name: 'ExportList',
            component: () => import('@/views/analytics/export/ExportList.vue'),
            meta: {
              titleKey: 'menu.exportRecords',
              title: 'Export Records',
              icon: FileOutput,
              order: 1,
              keepAlive: true,
            },
          },
        ],
      },
    ],
  },
]

export default analyticsRoutes
