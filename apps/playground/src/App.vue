<script setup lang="ts">
import {
  Button,
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
  Badge,
} from '@tabtab/ui'
</script>

<template>
  <div class="min-h-screen bg-background">
    <header class="border-b">
      <div class="container mx-auto px-4 py-4 flex items-center justify-between">
        <h1 class="text-2xl font-bold">TabTab UI Playground</h1>
        <Badge variant="secondary"> v0.0.1 </Badge>
      </div>
    </header>

    <main class="container mx-auto px-4 py-8">
      <div class="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle>Button 组件</CardTitle>
            <CardDescription>按钮组件示例</CardDescription>
          </CardHeader>
          <CardContent class="flex flex-wrap gap-2">
            <Button>Default</Button>
            <Button variant="secondary"> Secondary </Button>
            <Button variant="destructive"> Destructive </Button>
            <Button variant="outline"> Outline </Button>
            <Button variant="ghost"> Ghost </Button>
            <Button variant="link"> Link </Button>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Badge 组件</CardTitle>
            <CardDescription>徽章组件示例</CardDescription>
          </CardHeader>
          <CardContent class="flex flex-wrap gap-2">
            <Badge>Default</Badge>
            <Badge variant="secondary"> Secondary </Badge>
            <Badge variant="destructive"> Destructive </Badge>
            <Badge variant="outline"> Outline </Badge>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Card 组件</CardTitle>
            <CardDescription>卡片组件示例</CardDescription>
          </CardHeader>
          <CardContent>
            <p class="text-sm text-muted-foreground">这是一个卡片组件，用于展示内容块。</p>
          </CardContent>
        </Card>
      </div>
    </main>
  </div>
</template>
