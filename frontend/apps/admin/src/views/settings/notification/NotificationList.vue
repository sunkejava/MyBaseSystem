<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import {
  Bell,
  Check,
  CheckCheck,
  Trash2,
  X,
  Info,
  CheckCircle,
  AlertTriangle,
  MessageSquare,
} from 'lucide-vue-next'
import {
  useNotificationStore,
  type Notification,
  type NotificationType,
} from '@/stores/notification'
import { Button, ScrollArea, Card, CardContent } from '@tabtab/ui'

defineOptions({ name: 'NotificationList' })

const { t } = useI18n()
const notificationStore = useNotificationStore()
onMounted(()=>notificationStore.load())

/**
 * 通知类型配置
 */
const notificationTypeConfig: Record<
  NotificationType,
  { icon: typeof Info; color: string; bgColor: string }
> = {
  info: {
    icon: Info,
    color: 'text-blue-500',
    bgColor: 'bg-gradient-to-br from-blue-500/20 to-blue-600/10',
  },
  warning: {
    icon: AlertTriangle,
    color: 'text-amber-500',
    bgColor: 'bg-gradient-to-br from-amber-500/20 to-orange-600/10',
  },
  success: {
    icon: CheckCircle,
    color: 'text-emerald-500',
    bgColor: 'bg-gradient-to-br from-emerald-500/20 to-green-600/10',
  },
  error: {
    icon: AlertTriangle,
    color: 'text-red-500',
    bgColor: 'bg-gradient-to-br from-red-500/20 to-red-600/10',
  },
  message: {
    icon: MessageSquare,
    color: 'text-purple-500',
    bgColor: 'bg-gradient-to-br from-purple-500/20 to-violet-600/10',
  },
}

/**
 * 筛选类型
 */
type FilterType = 'all' | 'unread' | 'read'

/**
 * 筛选标签配置
 */
const filterTabs = [
  { key: 'all' as FilterType, label: 'common.notification.filterAll' },
  { key: 'unread' as FilterType, label: 'common.notification.filterUnread' },
  { key: 'read' as FilterType, label: 'common.notification.filterRead' },
]

/**
 * 类型筛选配置
 */
const typeFilters = [
  { key: 'all' as const, label: 'common.notification.filterTypeAll' },
  { key: 'info' as NotificationType, label: 'common.notification.filterTypeInfo' },
  { key: 'warning' as NotificationType, label: 'common.notification.filterTypeWarning' },
  { key: 'success' as NotificationType, label: 'common.notification.filterTypeSuccess' },
  { key: 'message' as NotificationType, label: 'common.notification.filterTypeMessage' },
]

const currentFilter = ref<FilterType>('all')
const currentTypeFilter = ref<NotificationType | 'all'>('all')

const notifications = computed(() => notificationStore.notifications)
const unreadCount = computed(() => notificationStore.unreadCount)
const typeCount = computed(() => notificationStore.typeCount)

/**
 * 筛选后的通知列表
 */
const filteredNotifications = computed(() => {
  let result = [...notifications.value]

  if (currentFilter.value === 'unread') {
    result = result.filter((n) => !n.read)
  } else if (currentFilter.value === 'read') {
    result = result.filter((n) => n.read)
  }

  if (currentTypeFilter.value !== 'all') {
    result = result.filter((n) => n.type === currentTypeFilter.value)
  }

  return result.sort((a, b) => b.createdAt.getTime() - a.createdAt.getTime())
})

/**
 * 分组后的通知
 */
const groupedNotifications = computed(() => {
  const now = new Date()
  const today = new Date(now.getFullYear(), now.getMonth(), now.getDate())
  const yesterday = new Date(today.getTime() - 24 * 60 * 60 * 1000)

  const groups = {
    unread: [] as Notification[],
    today: [] as Notification[],
    yesterday: [] as Notification[],
    earlier: [] as Notification[],
  }

  filteredNotifications.value.forEach((notification) => {
    const notifDate = new Date(
      notification.createdAt.getFullYear(),
      notification.createdAt.getMonth(),
      notification.createdAt.getDate(),
    )
    today.setHours(0, 0, 0, 0)

    if (!notification.read) {
      groups.unread.push(notification)
    } else if (notifDate.getTime() >= today.getTime()) {
      groups.today.push(notification)
    } else if (notifDate.getTime() >= yesterday.getTime()) {
      groups.yesterday.push(notification)
    } else {
      groups.earlier.push(notification)
    }
  })

  return groups
})

/**
 * 判断分组是否有内容
 */
function hasGroupItems(group: keyof typeof groupedNotifications.value): boolean {
  return groupedNotifications.value[group].length > 0
}

/**
 * 获取分组标题
 */
function getGroupTitle(group: keyof typeof groupedNotifications.value): string {
  const titles: Record<keyof typeof groupedNotifications.value, string> = {
    unread: t('common.notification.unread'),
    today: t('common.notification.today'),
    yesterday: t('common.notification.yesterday'),
    earlier: t('common.notification.earlier'),
  }
  return titles[group]
}

/**
 * 格式化时间
 */
function formatTime(date: Date): string {
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const minutes = Math.floor(diff / (1000 * 60))
  const hours = Math.floor(diff / (1000 * 60 * 60))
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))

  if (minutes < 1) {
    return t('common.notification.justNow')
  }
  if (minutes < 60) {
    return t('common.notification.minutesAgo', { minutes })
  }
  if (hours < 24) {
    return t('common.notification.hoursAgo', { hours })
  }
  if (days < 7) {
    return t('common.notification.daysAgo', { days })
  }
  return date.toLocaleDateString()
}

/**
 * 标记通知为已读
 */
function markAsRead(id: string) {
  notificationStore.markAsRead(id)
}

/**
 * 标记全部已读
 */
function markAllAsRead() {
  notificationStore.markAllAsRead()
}

/**
 * 删除通知
 */
function deleteNotification(id: string) {
  notificationStore.removeNotification(id)
}

/**
 * 清空所有通知
 */
function clearAllNotifications() {
  notificationStore.clearAll()
}

/**
 * 设置筛选类型
 */
function setFilter(filter: FilterType) {
  currentFilter.value = filter
}

/**
 * 设置类型筛选
 */
function setTypeFilter(type: NotificationType | 'all') {
  currentTypeFilter.value = type
}
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold">{{ t('settings.notificationList') }}</h1>
      <p class="text-muted-foreground mt-1">{{ t('settings.notificationListDesc') }}</p>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-4 gap-6">
      <div class="lg:col-span-3 space-y-4">
        <Card class="bg-muted/40 border border-border/50 rounded-xl">
          <CardContent class="p-4">
            <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
              <div class="flex items-center gap-2">
                <Button
                  v-for="tab in filterTabs"
                  :key="tab.key"
                  :variant="currentFilter === tab.key ? 'default' : 'ghost'"
                  size="sm"
                  class="h-8"
                  @click="setFilter(tab.key)"
                >
                  {{ t(tab.label) }}
                  <span
                    v-if="tab.key === 'unread' && unreadCount > 0"
                    class="ml-1.5 bg-primary-foreground/20 text-xs px-1.5 py-0.5 rounded-full"
                  >
                    {{ unreadCount }}
                  </span>
                </Button>
              </div>

              <div class="flex items-center gap-2">
                <Button
                  v-if="unreadCount > 0"
                  variant="outline"
                  size="sm"
                  class="h-8"
                  @click="markAllAsRead"
                >
                  <CheckCheck class="h-4 w-4 mr-1.5" />
                  {{ t('common.notification.markAllRead') }}
                </Button>
                <Button
                  v-if="notifications.length > 0"
                  variant="outline"
                  size="sm"
                  class="h-8 text-destructive hover:text-destructive"
                  @click="clearAllNotifications"
                >
                  <Trash2 class="h-4 w-4 mr-1.5" />
                  {{ t('common.notification.clearAll') }}
                </Button>
              </div>
            </div>

            <div class="flex flex-wrap items-center gap-2 mt-4 pt-4 border-t border-border/50">
              <span class="text-sm text-muted-foreground mr-2"
                >{{ t('common.notification.filterTypeLabel') }}：</span
              >
              <Button
                v-for="typeFilter in typeFilters"
                :key="typeFilter.key"
                :variant="currentTypeFilter === typeFilter.key ? 'secondary' : 'ghost'"
                size="sm"
                class="h-7 text-xs"
                @click="setTypeFilter(typeFilter.key)"
              >
                {{ t(typeFilter.label) }}
              </Button>
            </div>
          </CardContent>
        </Card>

        <Card class="bg-muted/40 border border-border/50 rounded-xl">
          <ScrollArea class="h-[calc(100vh-380px)] min-h-[400px]">
            <div
              v-if="filteredNotifications.length === 0"
              class="flex flex-col items-center justify-center py-16 text-center"
            >
              <div class="relative mb-4">
                <div
                  class="h-20 w-20 rounded-2xl bg-gradient-to-br from-muted to-muted/50 flex items-center justify-center shadow-sm"
                >
                  <Bell class="h-9 w-9 text-muted-foreground/60" />
                </div>
                <div
                  class="absolute -bottom-1 -right-1 h-6 w-6 rounded-full bg-background border-2 border-muted flex items-center justify-center"
                >
                  <Check class="h-3.5 w-3.5 text-muted-foreground/60" />
                </div>
              </div>
              <p class="text-base font-medium text-foreground/80 mb-1">
                {{ t('common.notification.empty') }}
              </p>
              <p class="text-sm text-muted-foreground">
                {{ t('common.notification.emptyDesc') }}
              </p>
            </div>

            <template v-else>
              <template v-if="hasGroupItems('unread')">
                <div class="px-4 py-2 bg-primary/5 border-b border-border/30">
                  <span class="text-sm font-semibold text-primary flex items-center gap-2">
                    <span class="h-2 w-2 rounded-full bg-primary animate-pulse" />
                    {{ getGroupTitle('unread') }}
                    <span class="bg-primary/20 text-primary text-xs px-2 py-0.5 rounded-full">
                      {{ groupedNotifications.unread.length }}
                    </span>
                  </span>
                </div>
                <div
                  v-for="notification in groupedNotifications.unread"
                  :key="notification.id"
                  class="group flex items-start gap-4 px-4 py-4 transition-all duration-200 cursor-pointer border-b border-border/20 relative overflow-hidden hover:bg-muted/50"
                  @click="markAsRead(notification.id)"
                >
                  <div
                    class="absolute left-0 top-0 bottom-0 w-[3px] bg-gradient-to-b from-primary to-primary/50"
                  />

                  <div
                    class="h-10 w-10 rounded-xl flex items-center justify-center flex-shrink-0 transition-all duration-300 relative z-10 shadow-sm ring-1 ring-black/5 dark:ring-white/5"
                    :class="notificationTypeConfig[notification.type].bgColor"
                  >
                    <component
                      :is="notificationTypeConfig[notification.type].icon"
                      class="h-5 w-5 transition-colors duration-200"
                      :class="notificationTypeConfig[notification.type].color"
                    />
                  </div>

                  <div class="flex-1 min-w-0 relative z-10">
                    <div class="flex items-start justify-between gap-2">
                      <p class="text-sm font-medium text-foreground truncate">
                        {{ notification.title }}
                      </p>
                      <span
                        class="text-xs text-muted-foreground flex-shrink-0 bg-muted/50 px-2 py-0.5 rounded"
                      >
                        {{ formatTime(notification.createdAt) }}
                      </span>
                    </div>
                    <p class="text-sm text-muted-foreground line-clamp-2 mt-1 leading-relaxed">
                      {{ notification.message }}
                    </p>
                  </div>

                  <div class="flex items-center gap-2 flex-shrink-0 relative z-10">
                    <div class="h-2.5 w-2.5 rounded-full bg-primary" />
                    <Button
                      variant="ghost"
                      size="icon"
                      class="h-8 w-8 opacity-0 group-hover:opacity-100 transition-all duration-200 text-muted-foreground hover:text-destructive hover:bg-destructive/10"
                      @click.stop="deleteNotification(notification.id)"
                    >
                      <X class="h-4 w-4" />
                    </Button>
                  </div>
                </div>
              </template>

              <template v-if="hasGroupItems('today')">
                <div class="px-4 py-2 bg-muted/30 border-b border-border/30">
                  <span class="text-sm font-medium text-muted-foreground flex items-center gap-2">
                    <span class="h-1.5 w-1.5 rounded-full bg-muted-foreground/50" />
                    {{ getGroupTitle('today') }}
                  </span>
                </div>
                <div
                  v-for="notification in groupedNotifications.today"
                  :key="notification.id"
                  class="group flex items-start gap-4 px-4 py-4 transition-all duration-200 cursor-pointer border-b border-border/20 relative overflow-hidden hover:bg-muted/50"
                  @click="markAsRead(notification.id)"
                >
                  <div
                    class="h-9 w-9 rounded-lg flex items-center justify-center flex-shrink-0 transition-all duration-200 relative z-10"
                    :class="notificationTypeConfig[notification.type].bgColor"
                  >
                    <component
                      :is="notificationTypeConfig[notification.type].icon"
                      class="h-4 w-4 transition-colors duration-200"
                      :class="notificationTypeConfig[notification.type].color"
                    />
                  </div>

                  <div class="flex-1 min-w-0 relative z-10">
                    <div class="flex items-start justify-between gap-2">
                      <p
                        class="text-sm text-foreground/80 truncate group-hover:text-foreground transition-colors"
                      >
                        {{ notification.title }}
                      </p>
                      <span class="text-xs text-muted-foreground flex-shrink-0">
                        {{ formatTime(notification.createdAt) }}
                      </span>
                    </div>
                    <p class="text-sm text-muted-foreground line-clamp-2 mt-1 leading-relaxed">
                      {{ notification.message }}
                    </p>
                  </div>

                  <Button
                    variant="ghost"
                    size="icon"
                    class="h-8 w-8 opacity-0 group-hover:opacity-100 transition-all duration-200 text-muted-foreground hover:text-destructive hover:bg-destructive/10 flex-shrink-0 relative z-10"
                    @click.stop="deleteNotification(notification.id)"
                  >
                    <X class="h-4 w-4" />
                  </Button>
                </div>
              </template>

              <template v-if="hasGroupItems('yesterday')">
                <div class="px-4 py-2 bg-muted/30 border-b border-border/30">
                  <span class="text-sm font-medium text-muted-foreground flex items-center gap-2">
                    <span class="h-1 w-1 rounded-full bg-muted-foreground/40" />
                    {{ getGroupTitle('yesterday') }}
                  </span>
                </div>
                <div
                  v-for="notification in groupedNotifications.yesterday"
                  :key="notification.id"
                  class="group flex items-start gap-4 px-4 py-4 transition-all duration-200 cursor-pointer border-b border-border/20 relative overflow-hidden hover:bg-muted/50"
                  @click="markAsRead(notification.id)"
                >
                  <div
                    class="h-9 w-9 rounded-lg flex items-center justify-center flex-shrink-0 transition-all duration-200 relative z-10 opacity-80"
                    :class="notificationTypeConfig[notification.type].bgColor"
                  >
                    <component
                      :is="notificationTypeConfig[notification.type].icon"
                      class="h-4 w-4 transition-colors duration-200"
                      :class="notificationTypeConfig[notification.type].color"
                    />
                  </div>

                  <div class="flex-1 min-w-0 relative z-10">
                    <div class="flex items-start justify-between gap-2">
                      <p
                        class="text-sm text-foreground/70 truncate group-hover:text-foreground/90 transition-colors"
                      >
                        {{ notification.title }}
                      </p>
                      <span class="text-xs text-muted-foreground flex-shrink-0">
                        {{ formatTime(notification.createdAt) }}
                      </span>
                    </div>
                    <p class="text-sm text-muted-foreground/80 line-clamp-2 mt-1 leading-relaxed">
                      {{ notification.message }}
                    </p>
                  </div>

                  <Button
                    variant="ghost"
                    size="icon"
                    class="h-8 w-8 opacity-0 group-hover:opacity-100 transition-all duration-200 text-muted-foreground hover:text-destructive hover:bg-destructive/10 flex-shrink-0 relative z-10"
                    @click.stop="deleteNotification(notification.id)"
                  >
                    <X class="h-4 w-4" />
                  </Button>
                </div>
              </template>

              <template v-if="hasGroupItems('earlier')">
                <div class="px-4 py-2 bg-muted/30 border-b border-border/30">
                  <span class="text-sm font-medium text-muted-foreground flex items-center gap-2">
                    <span class="h-1 w-1 rounded-full bg-muted-foreground/30" />
                    {{ getGroupTitle('earlier') }}
                  </span>
                </div>
                <div
                  v-for="notification in groupedNotifications.earlier"
                  :key="notification.id"
                  class="group flex items-start gap-4 px-4 py-4 transition-all duration-200 cursor-pointer border-b border-border/20 relative overflow-hidden hover:bg-muted/50"
                  @click="markAsRead(notification.id)"
                >
                  <div
                    class="h-9 w-9 rounded-lg flex items-center justify-center flex-shrink-0 transition-all duration-200 relative z-10 opacity-60"
                    :class="notificationTypeConfig[notification.type].bgColor"
                  >
                    <component
                      :is="notificationTypeConfig[notification.type].icon"
                      class="h-4 w-4 transition-colors duration-200"
                      :class="notificationTypeConfig[notification.type].color"
                    />
                  </div>

                  <div class="flex-1 min-w-0 relative z-10">
                    <div class="flex items-start justify-between gap-2">
                      <p
                        class="text-sm text-foreground/60 truncate group-hover:text-foreground/80 transition-colors"
                      >
                        {{ notification.title }}
                      </p>
                      <span class="text-xs text-muted-foreground flex-shrink-0">
                        {{ formatTime(notification.createdAt) }}
                      </span>
                    </div>
                    <p class="text-sm text-muted-foreground/70 line-clamp-2 mt-1 leading-relaxed">
                      {{ notification.message }}
                    </p>
                  </div>

                  <Button
                    variant="ghost"
                    size="icon"
                    class="h-8 w-8 opacity-0 group-hover:opacity-100 transition-all duration-200 text-muted-foreground hover:text-destructive hover:bg-destructive/10 flex-shrink-0 relative z-10"
                    @click.stop="deleteNotification(notification.id)"
                  >
                    <X class="h-4 w-4" />
                  </Button>
                </div>
              </template>
            </template>
          </ScrollArea>
        </Card>
      </div>

      <div class="space-y-4">
        <Card class="bg-muted/40 border border-border/50 rounded-xl">
          <CardContent class="p-4">
            <h3 class="text-sm font-semibold mb-3 flex items-center gap-2">
              <Bell class="h-4 w-4" />
              {{ t('common.notification.statistics') }}
            </h3>
            <div class="space-y-3">
              <div class="flex items-center justify-between">
                <span class="text-sm text-muted-foreground">{{
                  t('common.notification.total')
                }}</span>
                <span class="text-sm font-medium">{{ notifications.length }}</span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-muted-foreground">{{
                  t('common.notification.unreadCount')
                }}</span>
                <span class="text-sm font-medium text-primary">{{ unreadCount }}</span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-muted-foreground">{{
                  t('common.notification.readCount')
                }}</span>
                <span class="text-sm font-medium">{{ notifications.length - unreadCount }}</span>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card class="bg-muted/40 border border-border/50 rounded-xl">
          <CardContent class="p-4">
            <h3 class="text-sm font-semibold mb-3">
              {{ t('common.notification.typeDistribution') }}
            </h3>
            <div class="space-y-2">
              <div
                v-for="(config, type) in notificationTypeConfig"
                :key="type"
                class="flex items-center justify-between"
              >
                <div class="flex items-center gap-2">
                  <div
                    class="h-6 w-6 rounded-md flex items-center justify-center"
                    :class="config.bgColor"
                  >
                    <component :is="config.icon" class="h-3.5 w-3.5" :class="config.color" />
                  </div>
                  <span class="text-sm text-muted-foreground capitalize">{{
                    t(`common.notification.type${type.charAt(0).toUpperCase() + type.slice(1)}`)
                  }}</span>
                </div>
                <span class="text-sm font-medium">
                  {{ typeCount[type as NotificationType] }}
                </span>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  </div>
</template>
