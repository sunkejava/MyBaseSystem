<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
</script>

<template>
  <div>
    <h1 class="text-3xl font-bold">
      {{ t('settings.reportCenter') }}
    </h1>
    <p class="text-muted-foreground">
      {{ t('settings.reportCenterDesc') }}
    </p>
  </div>
</template>
