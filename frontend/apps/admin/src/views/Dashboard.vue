<script setup lang="ts">
import {
  Activity,
  AlertCircle,
  ArrowDownRight,
  ArrowRight,
  ArrowUpRight,
  DollarSign,
  Download,
  FileText,
  Plus,
  Settings,
  ShoppingCart,
  TrendingUp,
  Users,
} from 'lucide-vue-next'
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '@/stores/user'
import {
  Badge,
  Button,
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
  ScrollArea,
} from '@tabtab/ui'

const { t } = useI18n()
const userStore = useUserStore()

/**
 * 活动项类型
 */
interface ActivityItem {
  id: string
  type: 'success' | 'warning' | 'error' | 'info'
  title: string
  description?: string
  time: string
}

/**
 * 快捷操作类型
 */
interface QuickAction {
  id: string
  label: string
  icon: any
  onClick: () => void
  variant: 'default' | 'primary'
}

/**
 * 当前日期格式化
 */
const currentDate = computed(() => {
  const date = new Date()
  const options: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    weekday: 'long',
  }
  return date.toLocaleDateString('zh-CN', options)
})

/**
 * 问候语
 */
const greeting = computed(() => {
  const hour = new Date().getHours()
  if (hour < 12) return t('common.greeting.morning')
  if (hour < 18) return t('common.greeting.afternoon')
  return t('common.greeting.evening')
})

/**
 * 活动列表数据
 */
const activities = computed(() => [
  {
    id: '1',
    type: 'success' as const,
    title: t('dashboard.success'),
    description: `user@example.com ${t('dashboard.success')}`,
    time: t('dashboard.minutesAgo', { count: 2 }),
  },
  {
    id: '2',
    type: 'info' as const,
    title: t('dashboard.newOrders'),
    description: `#12345 ${t('dashboard.shipping')}`,
    time: t('dashboard.minutesAgo', { count: 15 }),
  },
  {
    id: '3',
    type: 'warning' as const,
    title: t('dashboard.inventoryWarning'),
    description: 'SKU-001',
    time: t('dashboard.hoursAgo', { count: 1 }),
  },
  {
    id: '4',
    type: 'success' as const,
    title: t('dashboard.success'),
    description: '$299.00',
    time: t('dashboard.hoursAgo', { count: 2 }),
  },
  {
    id: '5',
    type: 'error' as const,
    title: t('dashboard.error'),
    description: 'API',
    time: t('dashboard.hoursAgo', { count: 3 }),
  },
  {
    id: '6',
    type: 'success' as const,
    title: t('dashboard.pendingRefund'),
    description: '#12340',
    time: t('dashboard.hoursAgo', { count: 4 }),
  },
])

/**
 * 快捷操作列表
 */
const quickActions = computed(() => [
  {
    id: '1',
    label: t('dashboard.newOrder'),
    icon: ShoppingCart,
    onClick: () => {},
    variant: 'primary' as const,
  },
  {
    id: '2',
    label: t('dashboard.addUser'),
    icon: Users,
    onClick: () => {},
    variant: 'default' as const,
  },
  {
    id: '3',
    label: t('dashboard.generateReport'),
    icon: FileText,
    onClick: () => {},
    variant: 'default' as const,
  },
  {
    id: '4',
    label: t('dashboard.systemSettings'),
    icon: Settings,
    onClick: () => {},
    variant: 'default' as const,
  },
])

/**
 * 核心指标数据
 */
const metrics = computed(() => [
  {
    title: t('dashboard.metrics.totalUsers'),
    value: '12,847',
    change: 12.5,
    icon: Users,
    description: t('dashboard.activeUsersRatio', { ratio: 68 }),
    colorTheme: 'blue' as const,
    chartData: [45, 52, 48, 65, 72, 68, 75, 82, 78, 85, 88, 92],
  },
  {
    title: t('dashboard.metrics.totalRevenue'),
    value: '$84,230',
    change: 8.2,
    icon: DollarSign,
    description: t('dashboard.monthlyTargetComplete', { ratio: 92 }),
    colorTheme: 'green' as const,
    chartData: [60, 65, 58, 70, 75, 72, 80, 85, 82, 88, 90, 95],
  },
  {
    title: t('dashboard.metrics.totalOrders'),
    value: '3,421',
    change: -2.4,
    icon: ShoppingCart,
    description: t('dashboard.pendingOrders', { count: 156 }),
    colorTheme: 'orange' as const,
    chartData: [70, 68, 72, 65, 60, 58, 62, 65, 68, 70, 72, 75],
  },
  {
    title: t('dashboard.metrics.conversionRate'),
    value: '4.28%',
    change: 1.8,
    icon: TrendingUp,
    description: t('dashboard.aboveIndustry', { ratio: 15 }),
    colorTheme: 'purple' as const,
    chartData: [35, 38, 42, 40, 45, 48, 52, 50, 55, 58, 60, 65],
  },
])

/**
 * 系统状态数据
 */
const systemStatus = computed(() => [
  { name: t('dashboard.apiService'), status: 'running', uptime: '99.9%' },
  { name: t('dashboard.database'), status: 'running', uptime: '99.8%' },
  { name: t('dashboard.cacheService'), status: 'running', uptime: '99.9%' },
  { name: t('dashboard.messageQueue'), status: 'running', uptime: '99.5%' },
])

/**
 * 待处理事项
 */
const pendingTasks = computed(() => [
  {
    label: t('dashboard.inventoryWarning'),
    count: 5,
    color: 'text-amber-500',
    bgColor: 'bg-amber-500/10',
  },
  {
    label: t('dashboard.pendingReview'),
    count: 12,
    color: 'text-blue-500',
    bgColor: 'bg-blue-500/10',
  },
  {
    label: t('dashboard.pendingRefund'),
    count: 2,
    color: 'text-purple-500',
    bgColor: 'bg-purple-500/10',
  },
  {
    label: t('dashboard.systemNotification'),
    count: 8,
    color: 'text-cyan-500',
    bgColor: 'bg-cyan-500/10',
  },
])

/**
 * 色彩主题配置
 */
const colorThemes = {
  blue: {
    bg: 'bg-blue-500/10',
    icon: 'text-blue-500',
    chart: 'bg-blue-500',
  },
  green: {
    bg: 'bg-emerald-500/10',
    icon: 'text-emerald-500',
    chart: 'bg-emerald-500',
  },
  orange: {
    bg: 'bg-orange-500/10',
    icon: 'text-orange-500',
    chart: 'bg-orange-500',
  },
  purple: {
    bg: 'bg-violet-500/10',
    icon: 'text-violet-500',
    chart: 'bg-violet-500',
  },
}

/**
 * 活动类型样式配置
 */
const typeStyles = {
  success: { bg: 'bg-emerald-500/10', text: 'text-emerald-600', dot: 'bg-emerald-500' },
  warning: { bg: 'bg-amber-500/10', text: 'text-amber-600', dot: 'bg-amber-500' },
  error: { bg: 'bg-red-500/10', text: 'text-red-600', dot: 'bg-red-500' },
  info: { bg: 'bg-blue-500/10', text: 'text-blue-600', dot: 'bg-blue-500' },
}

/**
 * 生成折线路径
 */
function generateLinePath(data: number[], width: number, height: number): string {
  const max = Math.max(...data)
  const min = Math.min(...data)
  const range = max - min || 1
  const stepX = width / (data.length - 1)

  return data
    .map((value, index) => {
      const x = index * stepX
      const y = height - ((value - min) / range) * height
      return `${index === 0 ? 'M' : 'L'} ${x} ${y}`
    })
    .join(' ')
}

/**
 * 生成区域填充路径
 */
function generateAreaPath(data: number[], width: number, height: number): string {
  const linePath = generateLinePath(data, width, height)
  return `${linePath} L ${width} ${height} L 0 ${height} Z`
}

/**
 * 计算环形进度条参数
 */
function getProgressRingProps(progress: number, size: number, strokeWidth: number) {
  const radius = (size - strokeWidth) / 2
  const circumference = 2 * Math.PI * radius
  const safeProgress = Math.min(Math.max(progress, 0), 100)
  const strokeDashoffset = circumference - (safeProgress / 100) * circumference
  return { radius, circumference, strokeDashoffset }
}
</script>

<template>
  <div class="space-y-6">
    <!-- 页面标题区域 -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
      <div>
        <h1 class="text-2xl font-bold text-foreground">{{ t('dashboard.dashboard') }}</h1>
        <p class="text-muted-foreground mt-1">
          {{
            t('dashboard.dashboardWelcome', {
              name: userStore.user?.name || 'Admin',
              date: currentDate,
            })
          }}
        </p>
      </div>
      <div class="flex items-center gap-2">
        <Button variant="outline" size="sm" class="gap-2">
          <Download class="h-4 w-4" />
          <span class="hidden sm:inline">{{ t('common.export') }}</span>
        </Button>
        <Button variant="outline" size="sm" class="gap-2">
          <Plus class="h-4 w-4" />
          <span class="hidden sm:inline">{{ t('dashboard.quickEntry') }}</span>
        </Button>
      </div>
    </div>

    <!-- 核心指标卡片 -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <Card
        v-for="(metric, index) in metrics"
        :key="index"
        class="group relative overflow-hidden transition-all duration-300 hover:-translate-y-1"
      >
        <div
          class="absolute inset-0 bg-gradient-to-br opacity-50"
          :class="[
            `from-${metric.colorTheme === 'blue' ? 'blue' : metric.colorTheme === 'green' ? 'emerald' : metric.colorTheme === 'orange' ? 'orange' : 'violet'}-500/5 to-transparent`,
          ]"
        />
        <CardContent class="relative p-5">
          <div class="flex items-start justify-between">
            <!-- 左侧内容 -->
            <div class="flex-1 min-w-0">
              <!-- 图标和标题 -->
              <div class="flex items-center gap-2 mb-3">
                <div class="p-2 rounded-lg" :class="[colorThemes[metric.colorTheme].bg]">
                  <component
                    :is="metric.icon"
                    class="h-4 w-4"
                    :class="[colorThemes[metric.colorTheme].icon]"
                  />
                </div>
                <span class="text-sm font-medium text-muted-foreground">{{ metric.title }}</span>
              </div>

              <!-- 数值 -->
              <div class="text-2xl font-bold tracking-tight mb-1">
                {{ metric.value }}
              </div>

              <!-- 描述 -->
              <p class="text-xs text-muted-foreground truncate">
                {{ metric.description }}
              </p>

              <!-- 趋势 -->
              <div class="flex items-center gap-1 mt-2">
                <component
                  :is="metric.change >= 0 ? ArrowUpRight : ArrowDownRight"
                  class="h-3 w-3"
                  :class="[metric.change >= 0 ? 'text-emerald-500' : 'text-red-500']"
                />
                <span
                  class="text-xs font-medium"
                  :class="[metric.change >= 0 ? 'text-emerald-500' : 'text-red-500']"
                >
                  {{ Math.abs(metric.change) }}%
                </span>
                <span class="text-xs text-muted-foreground">{{ t('common.vsLastMonth') }}</span>
              </div>
            </div>

            <!-- 右侧图表 -->
            <div class="w-20 ml-3">
              <svg
                class="w-full h-[50px] overflow-visible"
                viewBox="0 0 100 50"
                preserveAspectRatio="none"
              >
                <defs>
                  <linearGradient :id="`gradient-${index}`" x1="0%" y1="0%" x2="0%" y2="100%">
                    <stop
                      offset="0%"
                      :class="colorThemes[metric.colorTheme].chart.replace('bg-', 'text-')"
                      stop-color="currentColor"
                      stop-opacity="0.3"
                    />
                    <stop
                      offset="100%"
                      :class="colorThemes[metric.colorTheme].chart.replace('bg-', 'text-')"
                      stop-color="currentColor"
                      stop-opacity="0"
                    />
                  </linearGradient>
                </defs>
                <path
                  :d="generateAreaPath(metric.chartData, 100, 50)"
                  :fill="`url(#gradient-${index})`"
                  class="transition-all duration-1000"
                />
                <path
                  :d="generateLinePath(metric.chartData, 100, 50)"
                  fill="none"
                  :class="[colorThemes[metric.colorTheme].chart.replace('bg-', 'stroke-')]"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  class="transition-all duration-1000 stroke-2"
                />
              </svg>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- 主要内容网格 -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- 左侧列 -->
      <div class="space-y-6">
        <!-- HeroCard - 本月业绩 -->
        <Card class="relative overflow-hidden border border-border/40 group">
          <!-- 渐变背景 -->
          <div
            class="absolute inset-0 bg-gradient-to-br from-primary via-primary/90 to-primary/70"
          />

          <!-- 装饰性光晕 -->
          <div class="absolute -top-24 -right-24 w-48 h-48 bg-white/10 rounded-full blur-3xl" />
          <div class="absolute -bottom-24 -left-24 w-48 h-48 bg-black/10 rounded-full blur-3xl" />

          <!-- 网格纹理 -->
          <div
            class="absolute inset-0 opacity-10"
            style="
              background-image: radial-gradient(circle at 1px 1px, white 1px, transparent 0);
              background-size: 20px 20px;
            "
          />

          <CardHeader class="relative pb-2">
            <div class="flex items-start justify-between">
              <div class="flex items-center gap-4">
                <!-- 图标容器 -->
                <div class="p-3 bg-white/20 backdrop-blur-sm rounded-lg">
                  <Activity class="h-8 w-8 text-white" />
                </div>
                <div>
                  <p class="text-white/70 text-sm font-medium">
                    {{ t('dashboard.monthlyPerformance') }}
                  </p>
                  <CardTitle class="text-3xl font-bold text-white mt-1"> $84,230 </CardTitle>
                </div>
              </div>

              <!-- 趋势徽章 -->
              <div
                class="flex items-center gap-1 px-3 py-1.5 backdrop-blur-sm rounded-md bg-emerald-500/20"
              >
                <TrendingUp class="h-4 w-4 text-white" />
                <span class="text-sm font-semibold text-white">12.5%</span>
              </div>
            </div>

            <p class="text-white/80 text-sm mt-3">
              {{ t('dashboard.realTimeMonitor') }}
            </p>
          </CardHeader>

          <CardContent class="relative space-y-4">
            <div class="grid grid-cols-3 gap-3 mt-2">
              <div class="text-center p-3 bg-white/10 backdrop-blur-sm rounded-lg">
                <p class="text-xs text-white/70">
                  {{ t('dashboard.thisWeekRevenue') }}
                </p>
                <p class="text-lg font-semibold text-white">$18.5k</p>
              </div>
              <div class="text-center p-3 bg-white/10 backdrop-blur-sm rounded-lg">
                <p class="text-xs text-white/70">
                  {{ t('dashboard.thisWeekOrders') }}
                </p>
                <p class="text-lg font-semibold text-white">856</p>
              </div>
              <div class="text-center p-3 bg-white/10 backdrop-blur-sm rounded-lg">
                <p class="text-xs text-white/70">
                  {{ t('dashboard.newCustomers') }}
                </p>
                <p class="text-lg font-semibold text-white">128</p>
              </div>
            </div>

            <!-- 操作按钮 -->
            <Button
              variant="secondary"
              size="default"
              class="w-full bg-white/20 hover:bg-white/30 text-white border-0 backdrop-blur-sm group/btn"
            >
              {{ t('dashboard.viewDetails') }}
              <ArrowRight class="h-4 w-4 ml-2 transition-transform group-hover/btn:translate-x-1" />
            </Button>
          </CardContent>
        </Card>

        <!-- 最近活动 -->
        <Card class="border border-border/40 rounded-xl">
          <CardHeader class="pb-3">
            <CardTitle class="text-base font-semibold">
              {{ t('dashboard.recentActivities') }}
            </CardTitle>
          </CardHeader>
          <CardContent class="pt-0">
            <ScrollArea class="h-[280px] pr-4">
              <div class="space-y-3">
                <div
                  v-for="(item, index) in activities.slice(0, 6)"
                  :key="item.id"
                  class="group flex items-start gap-3 p-3 transition-all duration-200 hover:bg-muted/50 cursor-pointer rounded-lg"
                  :style="{ animationDelay: `${index * 50}ms` }"
                >
                  <!-- 状态指示点 -->
                  <div
                    class="w-2 h-2 mt-2 rounded-full flex-shrink-0"
                    :class="[typeStyles[item.type].dot]"
                  />

                  <!-- 内容区域 -->
                  <div class="flex-1 min-w-0">
                    <div class="flex items-center justify-between gap-2">
                      <p
                        class="text-sm font-medium truncate group-hover:text-primary transition-colors"
                      >
                        {{ item.title }}
                      </p>
                      <Badge
                        class="text-xs flex-shrink-0"
                        :class="[typeStyles[item.type].bg, typeStyles[item.type].text]"
                        variant="outline"
                      >
                        {{
                          item.type === 'success'
                            ? t('dashboard.success')
                            : item.type === 'warning'
                              ? t('dashboard.warning')
                              : item.type === 'error'
                                ? t('dashboard.error')
                                : t('dashboard.info')
                        }}
                      </Badge>
                    </div>
                    <p v-if="item.description" class="text-xs text-muted-foreground mt-1 truncate">
                      {{ item.description }}
                    </p>
                    <p class="text-xs text-muted-foreground/70 mt-1.5">
                      {{ item.time }}
                    </p>
                  </div>
                </div>
              </div>
            </ScrollArea>
          </CardContent>
        </Card>

        <!-- 本周趋势 -->
        <Card class="bg-muted/40 border border-border/50 rounded-xl">
          <CardHeader class="pb-4">
            <div class="flex items-center justify-between">
              <div>
                <CardTitle class="text-base">
                  {{ t('dashboard.weeklyTrend') }}
                </CardTitle>
                <CardDescription>{{ t('dashboard.weeklyTrendDesc') }}</CardDescription>
              </div>
              <Badge variant="default" class="gap-1">
                <TrendingUp class="h-3 w-3" />
                +5.3%
              </Badge>
            </div>
          </CardHeader>
          <CardContent class="pt-0">
            <!-- 柱状图 -->
            <div class="flex items-end justify-between gap-1 h-[80px]">
              <div
                v-for="(value, idx) in [45, 52, 48, 65, 72, 68, 75]"
                :key="idx"
                class="bg-primary rounded-t transition-all duration-700 ease-out"
                :style="{
                  width: `${100 / 7}%`,
                  height: `${value}%`,
                  animationDelay: `${idx * 100}ms`,
                }"
              />
            </div>
            <div class="flex justify-between text-xs text-muted-foreground mt-3">
              <span>{{ t('dashboard.mon') }}</span>
              <span>{{ t('dashboard.wed') }}</span>
              <span>{{ t('dashboard.fri') }}</span>
              <span>{{ t('dashboard.sun') }}</span>
            </div>
          </CardContent>
        </Card>
      </div>

      <!-- 中间列 -->
      <div class="space-y-6">
        <!-- 快捷操作 -->
        <Card class="border border-border/40">
          <CardHeader class="pb-3">
            <CardTitle class="text-base font-semibold">
              {{ t('dashboard.quickEntry') }}
            </CardTitle>
          </CardHeader>
          <CardContent class="pt-0">
            <div class="grid grid-cols-2 gap-3">
              <Button
                v-for="(action, index) in quickActions"
                :key="action.id"
                class="h-auto py-4 px-3 flex flex-col items-center gap-2 rounded-xl transition-all duration-200 border-0 hover:-translate-y-0.5"
                :class="[
                  action.variant === 'primary'
                    ? 'bg-primary hover:bg-primary/90 text-primary-foreground'
                    : 'bg-muted hover:bg-muted/80 text-muted-foreground hover:text-foreground',
                ]"
                :style="{ animationDelay: `${index * 50}ms` }"
                @click="action.onClick"
              >
                <div
                  v-if="action.icon"
                  class="p-2 rounded-lg"
                  :class="[action.variant === 'primary' ? 'bg-white/20' : 'bg-background/50']"
                >
                  <component :is="action.icon" class="h-5 w-5" />
                </div>
                <span class="text-sm font-medium">{{ action.label }}</span>
              </Button>
            </div>
          </CardContent>
        </Card>

        <!-- 系统状态 -->
        <Card class="bg-muted/40 border border-border/50 rounded-xl">
          <CardHeader class="pb-4">
            <div class="flex items-center justify-between">
              <CardTitle class="text-base">
                {{ t('dashboard.systemStatus') }}
              </CardTitle>
              <Badge
                variant="outline"
                class="text-emerald-500 border-emerald-500/20 bg-emerald-500/10"
              >
                <div class="w-1.5 h-1.5 bg-emerald-500 mr-1.5 animate-pulse rounded-full" />
                {{ t('dashboard.operational') }}
              </Badge>
            </div>
          </CardHeader>
          <CardContent class="pt-0">
            <div class="space-y-3">
              <div
                v-for="service in systemStatus"
                :key="service.name"
                class="flex items-center justify-between p-3 bg-muted/50 hover:bg-muted transition-colors rounded-lg"
              >
                <div class="flex items-center gap-3">
                  <div class="w-2 h-2 bg-emerald-500 rounded-full" />
                  <span class="text-sm font-medium">{{ service.name }}</span>
                </div>
                <span class="text-xs text-muted-foreground">{{
                  t('dashboard.availableRate', { uptime: service.uptime })
                }}</span>
              </div>
            </div>
          </CardContent>
        </Card>

        <!-- 订单状态分布 -->
        <Card class="bg-muted/40 border border-border/50 rounded-xl">
          <CardHeader class="pb-4">
            <CardTitle class="text-base">
              {{ t('dashboard.orderStatus') }}
            </CardTitle>
          </CardHeader>
          <CardContent class="pt-0">
            <div class="flex items-center justify-center mb-4">
              <!-- 环形进度条 -->
              <div class="relative inline-flex items-center justify-center w-[100px] h-[100px]">
                <svg class="transform -rotate-90" width="100" height="100">
                  <!-- 背景圆环 -->
                  <circle
                    cx="50"
                    cy="50"
                    r="45"
                    fill="none"
                    class="text-muted/20"
                    stroke-width="10"
                    stroke="currentColor"
                  />
                  <!-- 进度圆环 -->
                  <circle
                    cx="50"
                    cy="50"
                    r="45"
                    fill="none"
                    class="text-primary transition-all duration-1000 ease-out"
                    stroke-width="10"
                    stroke="currentColor"
                    stroke-linecap="round"
                    :stroke-dasharray="2 * Math.PI * 45"
                    :stroke-dashoffset="2 * Math.PI * 45 - (75 / 100) * 2 * Math.PI * 45"
                  />
                </svg>
                <div class="absolute inset-0 flex flex-col items-center justify-center">
                  <span class="text-lg font-bold">75%</span>
                </div>
              </div>
            </div>
            <div class="grid grid-cols-2 gap-3">
              <div class="text-center p-2 bg-muted/50 rounded-lg">
                <p class="text-lg font-semibold">28</p>
                <p class="text-xs text-muted-foreground">
                  {{ t('dashboard.newOrders') }}
                </p>
              </div>
              <div class="text-center p-2 bg-muted/50 rounded-lg">
                <p class="text-lg font-semibold">15</p>
                <p class="text-xs text-muted-foreground">
                  {{ t('dashboard.processing') }}
                </p>
              </div>
              <div class="text-center p-2 bg-muted/50 rounded-lg">
                <p class="text-lg font-semibold">8</p>
                <p class="text-xs text-muted-foreground">
                  {{ t('dashboard.shipping') }}
                </p>
              </div>
              <div class="text-center p-2 bg-muted/50 rounded-lg">
                <p class="text-lg font-semibold">42</p>
                <p class="text-xs text-muted-foreground">
                  {{ t('dashboard.completed') }}
                </p>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <!-- 右侧列 -->
      <div class="space-y-6">
        <!-- 待处理事项 -->
        <Card class="bg-muted/40 border border-border/50 rounded-xl">
          <CardHeader class="pb-4">
            <div class="flex items-center justify-between">
              <CardTitle class="text-base">
                {{ t('dashboard.todoItems') }}
              </CardTitle>
              <Badge variant="secondary">
                {{ pendingTasks.reduce((acc, t) => acc + t.count, 0) }}
              </Badge>
            </div>
          </CardHeader>
          <CardContent class="pt-0">
            <div class="space-y-2">
              <div
                v-for="task in pendingTasks"
                :key="task.label"
                class="flex items-center justify-between p-3 hover:bg-muted/50 transition-colors cursor-pointer group rounded-lg"
              >
                <div class="flex items-center gap-3">
                  <div
                    class="w-8 h-8 flex items-center justify-center rounded-lg"
                    :class="[task.bgColor]"
                  >
                    <AlertCircle class="h-4 w-4" :class="[task.color]" />
                  </div>
                  <span class="text-sm">{{ task.label }}</span>
                </div>
                <div class="flex items-center gap-2">
                  <span class="text-sm font-semibold" :class="[task.color]">{{ task.count }}</span>
                  <ArrowUpRight
                    class="h-4 w-4 text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity"
                  />
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        <!-- 处理效率 -->
        <Card class="bg-muted/40 border border-border/50 rounded-xl">
          <CardHeader class="pb-4">
            <CardTitle class="text-base">
              {{ t('dashboard.avgProcessingTime') }}
            </CardTitle>
          </CardHeader>
          <CardContent class="pt-0">
            <div class="space-y-4">
              <div>
                <div class="flex justify-between text-sm mb-2">
                  <span class="text-muted-foreground">{{ t('dashboard.paymentProcessing') }}</span>
                  <span class="font-medium">{{ t('dashboard.seconds', { count: 2.3 }) }}</span>
                </div>
                <div class="h-2 bg-muted overflow-hidden rounded-full">
                  <div
                    class="h-full bg-emerald-500 transition-all duration-1000 rounded-full"
                    style="width: 85%"
                  />
                </div>
              </div>
              <div>
                <div class="flex justify-between text-sm mb-2">
                  <span class="text-muted-foreground">{{ t('dashboard.orderConfirmation') }}</span>
                  <span class="font-medium">{{ t('dashboard.seconds', { count: 5.1 }) }}</span>
                </div>
                <div class="h-2 bg-muted overflow-hidden rounded-full">
                  <div
                    class="h-full bg-blue-500 transition-all duration-1000 rounded-full"
                    style="width: 70%"
                  />
                </div>
              </div>
              <div>
                <div class="flex justify-between text-sm mb-2">
                  <span class="text-muted-foreground">{{ t('dashboard.shippingProcessing') }}</span>
                  <span class="font-medium">{{ t('dashboard.minutes', { count: 12.5 }) }}</span>
                </div>
                <div class="h-2 bg-muted overflow-hidden rounded-full">
                  <div
                    class="h-full bg-amber-500 transition-all duration-1000 rounded-full"
                    style="width: 60%"
                  />
                </div>
              </div>
              <div>
                <div class="flex justify-between text-sm mb-2">
                  <span class="text-muted-foreground">{{ t('dashboard.customerResponse') }}</span>
                  <span class="font-medium">{{ t('dashboard.minutes', { count: 3.2 }) }}</span>
                </div>
                <div class="h-2 bg-muted overflow-hidden rounded-full">
                  <div
                    class="h-full bg-purple-500 transition-all duration-1000 rounded-full"
                    style="width: 90%"
                  />
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  </div>
</template>
