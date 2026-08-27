<script setup lang="ts">
import { computed } from 'vue'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@tabtab/ui'
import { Label } from '@tabtab/ui'
import { Button } from '@tabtab/ui'
import { useThemeStore, type ThemeMode } from '@/stores/theme'
import type { LayoutMode, LayoutVariant } from '@tabtab/ui'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
const router = useRouter()

const themeStore = useThemeStore()

const themeOptions = computed(() => [
  {
    value: 'light' as ThemeMode,
    label: t('settings.lightMode'),
    description: t('settings.lightModeDesc'),
  },
  {
    value: 'dark' as ThemeMode,
    label: t('settings.darkMode'),
    description: t('settings.darkModeDesc'),
  },
  {
    value: 'system' as ThemeMode,
    label: t('settings.systemMode'),
    description: t('settings.systemModeDesc'),
  },
])

const layoutOptions = computed(() => [
  {
    value: 'sidebar' as LayoutMode,
    label: t('settings.sidebar'),
    description: t('settings.sidebarDesc'),
  },
  {
    value: 'top-nav' as LayoutMode,
    label: t('settings.topNav'),
    description: t('settings.topNavDesc'),
  },
  {
    value: 'mixed' as LayoutMode,
    label: t('settings.mixed'),
    description: t('settings.mixedDesc'),
  },
  {
    value: 'double-sidebar' as LayoutMode,
    label: t('settings.doubleSidebar'),
    description: t('settings.doubleSidebarDesc'),
  },
  {
    value: 'mixed-double' as LayoutMode,
    label: t('settings.mixedDouble'),
    description: t('settings.mixedDoubleDesc'),
  },
])

const variantOptions = computed(() => [
  {
    value: 'fixed' as LayoutVariant,
    label: t('settings.fixedWidth'),
    description: t('settings.fixedWidthDesc'),
  },
  {
    value: 'fluid' as LayoutVariant,
    label: t('settings.fluid'),
    description: t('settings.fluidDesc'),
  },
  {
    value: 'compact' as LayoutVariant,
    label: t('settings.compact'),
    description: t('settings.compactDesc'),
  },
])
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold">
        {{ t('settings.settings') }}
      </h1>
      <p class="text-muted-foreground">
        {{ t('settings.settingsSubtitle') }}
      </p>
    </div>

    <Card>
      <CardHeader>
        <CardTitle>{{ t('settings.theme') }}</CardTitle>
        <CardDescription>{{ t('settings.themeDescription') }}</CardDescription>
      </CardHeader>
      <CardContent class="space-y-4">
        <div class="space-y-3">
          <Label>{{ t('settings.themeMode') }}</Label>
          <div class="grid grid-cols-3 gap-4">
            <Button
              v-for="option in themeOptions"
              :key="option.value"
              :variant="themeStore.themeMode === option.value ? 'default' : 'outline'"
              class="h-auto flex-col items-start gap-1 p-4"
              @click="themeStore.setThemeMode(option.value)"
            >
              <span class="font-medium">{{ option.label }}</span>
              <span class="text-xs text-muted-foreground font-normal">{{
                option.description
              }}</span>
            </Button>
          </div>
        </div>
      </CardContent>
    </Card>

    <Card>
      <CardHeader>
        <CardTitle>{{ t('settings.layout') }}</CardTitle>
        <CardDescription>{{ t('settings.layoutDescription') }}</CardDescription>
      </CardHeader>
      <CardContent class="space-y-4">
        <div class="space-y-3">
          <Label>{{ t('settings.layoutMode') }}</Label>
          <div class="grid grid-cols-3 gap-4">
            <Button
              v-for="option in layoutOptions"
              :key="option.value"
              :variant="themeStore.layoutMode === option.value ? 'default' : 'outline'"
              class="h-auto flex-col items-start gap-1 p-4"
              @click="themeStore.setLayoutMode(option.value)"
            >
              <span class="font-medium">{{ option.label }}</span>
              <span class="text-xs text-muted-foreground font-normal">{{
                option.description
              }}</span>
            </Button>
          </div>
        </div>

        <div class="space-y-2">
          <Label>{{ t('settings.layoutVariant') }}</Label>
          <div class="grid grid-cols-3 gap-4">
            <Button
              v-for="option in variantOptions"
              :key="option.value"
              :variant="themeStore.layoutVariant === option.value ? 'default' : 'outline'"
              class="h-auto flex-col items-start gap-1 p-4"
              @click="themeStore.setLayoutVariant(option.value)"
            >
              <span class="font-medium">{{ option.label }}</span>
              <span class="text-xs text-muted-foreground font-normal">{{
                option.description
              }}</span>
            </Button>
          </div>
        </div>

        <div class="pt-4 border-t">
          <Button variant="outline" @click="router.push('/settings/appearance/layout')">
            {{ t('settings.moreLayoutSettings') }}
          </Button>
        </div>
      </CardContent>
    </Card>
  </div>
</template>
