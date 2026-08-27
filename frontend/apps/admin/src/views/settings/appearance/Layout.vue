<script setup lang="ts">
import { useThemeStore } from '@/stores/theme'
import { Switch, Button } from '@tabtab/ui'
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
const router = useRouter()

defineOptions({ name: 'SettingsLayout' })

const themeStore = useThemeStore()

const layoutOptions = computed(() => [
  {
    value: 'sidebar',
    label: t('settings.sidebarMode'),
    description: t('settings.sidebarModeDesc'),
  },
  { value: 'top-nav', label: t('settings.topNavMode'), description: t('settings.topNavModeDesc') },
  { value: 'mixed', label: t('settings.mixedMode'), description: t('settings.mixedModeDesc') },
  {
    value: 'double-sidebar',
    label: t('settings.doubleSidebarMode'),
    description: t('settings.doubleSidebarModeDesc'),
  },
  {
    value: 'mixed-double',
    label: t('settings.mixedDoubleMode'),
    description: t('settings.mixedDoubleModeDesc'),
  },
])

const variantOptions = computed(() => [
  { value: 'fixed', label: t('settings.fixedWidth'), description: t('settings.fixedWidthDesc') },
  { value: 'fluid', label: t('settings.fluidWidth'), description: t('settings.fluidWidthDesc') },
  {
    value: 'compact',
    label: t('settings.compactMode'),
    description: t('settings.compactModeDesc'),
  },
  {
    value: 'streamer',
    label: t('settings.streamerMode'),
    description: t('settings.streamerModeDesc'),
  },
])

const currentLayout = computed({
  get: () => themeStore.layoutMode,
  set: (value) => themeStore.setLayoutMode(value),
})

const currentVariant = computed({
  get: () => themeStore.layoutVariant,
  set: (value) => themeStore.setLayoutVariant(value),
})

const showTabs = computed({
  get: () => themeStore.showTabs,
  set: (value) => themeStore.setShowTabs(value),
})

const tabsFixed = computed({
  get: () => themeStore.tabsFixed,
  set: (value) => themeStore.setTabsFixed(value),
})
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold">{{ t('settings.layoutSettings') }}</h1>
      <p class="text-muted-foreground">{{ t('settings.layoutSettingsDesc') }}</p>
    </div>

    <div class="flex gap-2 pb-4 border-b">
      <Button variant="outline" size="sm" @click="router.push('/settings/appearance/theme')">
        {{ t('settings.theme') }}
      </Button>
      <Button variant="default" size="sm">
        {{ t('settings.layoutSettings') }}
      </Button>
    </div>

    <div class="space-y-4">
      <div class="space-y-2">
        <h2 class="text-lg font-semibold">{{ t('settings.layoutPattern') }}</h2>
        <p class="text-sm text-muted-foreground">{{ t('settings.layoutPatternDesc') }}</p>
        <div class="grid gap-3 mt-3">
          <div
            v-for="option in layoutOptions"
            :key="option.value"
            class="flex items-start gap-3 p-4 rounded-lg border cursor-pointer transition-colors hover:bg-accent"
            :class="
              currentLayout === option.value ? 'border-primary bg-accent/50' : 'border-border'
            "
            @click="currentLayout = option.value"
          >
            <div class="flex-1">
              <div class="font-medium">{{ option.label }}</div>
              <div class="text-sm text-muted-foreground">{{ option.description }}</div>
            </div>
            <div
              class="h-4 w-4 rounded-full border-2 flex items-center justify-center"
              :class="currentLayout === option.value ? 'border-primary' : 'border-muted-foreground'"
            >
              <div v-if="currentLayout === option.value" class="h-2 w-2 rounded-full bg-primary" />
            </div>
          </div>
        </div>
      </div>

      <div class="space-y-2">
        <h2 class="text-lg font-semibold">{{ t('settings.layoutVariantInner') }}</h2>
        <p class="text-sm text-muted-foreground">{{ t('settings.layoutVariantInnerDesc') }}</p>
        <div class="grid gap-3 mt-3">
          <div
            v-for="option in variantOptions"
            :key="option.value"
            class="flex items-start gap-3 p-4 rounded-lg border cursor-pointer transition-colors hover:bg-accent"
            :class="
              currentVariant === option.value ? 'border-primary bg-accent/50' : 'border-border'
            "
            @click="currentVariant = option.value"
          >
            <div class="flex-1">
              <div class="font-medium">{{ option.label }}</div>
              <div class="text-sm text-muted-foreground">{{ option.description }}</div>
            </div>
            <div
              class="h-4 w-4 rounded-full border-2 flex items-center justify-center"
              :class="
                currentVariant === option.value ? 'border-primary' : 'border-muted-foreground'
              "
            >
              <div v-if="currentVariant === option.value" class="h-2 w-2 rounded-full bg-primary" />
            </div>
          </div>
        </div>
      </div>

      <div class="space-y-2">
        <h2 class="text-lg font-semibold">{{ t('settings.tabsSettingsInner') }}</h2>
        <p class="text-sm text-muted-foreground">{{ t('settings.tabsSettingsInnerDesc') }}</p>
        <div class="mt-3 space-y-4">
          <div class="flex items-center justify-between p-4 rounded-lg border border-border">
            <div class="space-y-0.5">
              <div class="font-medium">{{ t('settings.showTabsLabel') }}</div>
              <div class="text-sm text-muted-foreground">{{ t('settings.showTabsLabelDesc') }}</div>
            </div>
            <Switch :checked="showTabs" @update:checked="showTabs = $event" />
          </div>
          <div class="flex items-center justify-between p-4 rounded-lg border border-border">
            <div class="space-y-0.5">
              <div class="font-medium">{{ t('settings.tabsFixedLabel') }}</div>
              <div class="text-sm text-muted-foreground">
                {{ t('settings.tabsFixedLabelDesc') }}
              </div>
            </div>
            <Switch
              :checked="tabsFixed"
              :disabled="!showTabs"
              @update:checked="tabsFixed = $event"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
