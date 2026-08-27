<script setup lang="ts">
import { useRoute } from 'vue-router'

const route = useRoute()
const componentName = route.params.name as string
</script>

<template>
  <div>
    <h1>Component: {{ componentName }}</h1>
  </div>
</template>
