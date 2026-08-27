<script setup lang="ts">
import { Button } from '@tabtab/ui'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
const router = useRouter()

defineOptions({ name: 'SettingsTheme' })
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold">{{ t('settings.theme') }}</h1>
      <p class="text-muted-foreground">{{ t('settings.themeDescription') }}</p>
    </div>

    <div class="flex gap-2 pb-4 border-b">
      <Button variant="default" size="sm">
        {{ t('settings.theme') }}
      </Button>
      <Button variant="outline" size="sm" @click="router.push('/settings/appearance/layout')">
        {{ t('settings.layoutSettings') }}
      </Button>
    </div>

    <div class="text-center text-muted-foreground py-12">
      {{ t('settings.openThemeSettings') }}
    </div>
  </div>
</template>
