<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
defineOptions({ name: 'ExportList' })
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold">{{ t('settings.exportRecords') }}</h1>
      <p class="text-muted-foreground">{{ t('settings.exportRecordsDesc') }}</p>
    </div>
    <div class="text-center text-muted-foreground py-12">
      {{ t('settings.exportRecordsPage') }}
    </div>
  </div>
</template>
