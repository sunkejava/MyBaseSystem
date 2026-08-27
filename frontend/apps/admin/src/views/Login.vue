<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '@/stores/user'
import { useThemeStore } from '@/stores/theme'
import { Button, Input, Label, Checkbox } from '@tabtab/ui'
import {
  ArrowRight,
  BarChart3,
  CheckCircle2,
  Eye,
  EyeOff,
  Layers,
  Loader2,
  Lock,
  Mail,
  Moon,
  Sparkles,
  Sun,
  Users,
  Zap,
} from 'lucide-vue-next'

const { t } = useI18n()
const router = useRouter()
const userStore = useUserStore()
const themeStore = useThemeStore()

const email = ref('admin')
const password = ref('Admin123!')
const showPassword = ref(false)
const isLoading = ref(false)
const rememberMe = ref(true)
const errorMessage = ref('')

const passwordType = computed(() => (showPassword.value ? 'text' : 'password'))

const features = computed(() => [
  { icon: BarChart3, text: t('login.features.realtimeAnalytics') },
  { icon: Users, text: t('login.features.teamCollaboration') },
  { icon: Layers, text: t('login.features.modularArchitecture') },
  { icon: Zap, text: t('login.features.highPerformance') },
])

function togglePasswordVisibility() {
  showPassword.value = !showPassword.value
}

async function handleLogin() {
  if (isLoading.value) return
  if (!email.value || !password.value) {
    errorMessage.value = t('login.pleaseInputCredentials')
    return
  }

  errorMessage.value = ''
  isLoading.value = true

  try {
    await userStore.loginWithPassword(email.value, password.value, rememberMe.value)
    const redirect = router.currentRoute.value.query.redirect as string | undefined
    router.push(redirect || { name: 'Dashboard' })
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : t('login.loginFailed')
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <div class="relative min-h-screen w-full flex bg-background overflow-hidden">
    <div class="hidden lg:flex lg:w-1/2 xl:w-[55%] relative overflow-hidden">
      <div
        class="absolute inset-0 bg-gradient-to-br from-primary via-primary/95 to-primary/75 transition-colors duration-500"
        :style="{
          background: `linear-gradient(135deg, ${themeStore.currentColors?.primary || 'oklch(0.205 0 0)'} 0%, ${themeStore.currentColors?.primary || 'oklch(0.205 0 0)'}ee 50%, ${themeStore.currentColors?.primary || 'oklch(0.205 0 0)'}cc 100%)`,
        }"
      >
        <div
          class="absolute top-0 left-0 w-[600px] h-[600px] bg-white/10 rounded-full blur-[120px] -translate-x-1/2 -translate-y-1/2 animate-pulse"
        />
        <div
          class="absolute bottom-0 right-0 w-[500px] h-[500px] bg-white/5 rounded-full blur-[100px] translate-x-1/3 translate-y-1/3"
        />
        <div
          class="absolute top-1/2 left-1/2 w-[400px] h-[400px] bg-white/5 rounded-full blur-[80px] -translate-x-1/2 -translate-y-1/2"
        />
      </div>

      <div
        class="absolute inset-0 bg-[linear-gradient(rgba(255,255,255,0.03)_1px,transparent_1px),linear-gradient(90deg,rgba(255,255,255,0.03)_1px,transparent_1px)] bg-[size:60px_60px]"
      />

      <div class="relative z-10 flex flex-col justify-between w-full p-12 xl:p-16">
        <div class="flex items-center gap-3 animate-fade-in">
          <div
            class="flex items-center justify-center w-12 h-12 bg-white/10 backdrop-blur-sm border border-white/20 shadow-lg"
            :style="{ borderRadius: `calc(var(--radius) * 1.5)` }"
          >
            <span class="text-2xl font-bold text-white">T</span>
          </div>
          <span class="text-xl font-bold text-white tracking-tight">MY BASE SYSTEM</span>
        </div>

        <div class="space-y-8 animate-fade-in-up">
          <div class="space-y-4">
            <div
              class="inline-flex items-center gap-2 px-3 py-1.5 bg-white/10 backdrop-blur-sm border border-white/20 text-white/90 text-sm"
              :style="{ borderRadius: `calc(var(--radius) * 2)` }"
            >
              <Sparkles class="w-4 h-4" />
              <span>{{ t('login.newVersion') }}</span>
            </div>
            <h1 class="text-4xl xl:text-5xl font-bold text-white leading-tight">
              {{ t('login.smart') }}<br />
              <span class="text-white/80">{{ t('login.backendSystem') }}</span>
            </h1>
            <p class="text-lg text-white/70 max-w-md leading-relaxed">
              {{ t('login.description') }}
            </p>
          </div>

          <div class="grid grid-cols-2 gap-4 max-w-md">
            <div
              v-for="(feature, index) in features"
              :key="index"
              class="flex items-center gap-3 p-3 bg-white/5 backdrop-blur-sm border border-white/10 hover:bg-white/10 transition-all duration-300 hover:scale-[1.02]"
              :style="{ borderRadius: `calc(var(--radius) * 1.5)` }"
              :class="`animate-fade-in-up animation-delay-${index + 1}`"
            >
              <div
                class="flex items-center justify-center w-8 h-8 bg-white/10"
                :style="{ borderRadius: `calc(var(--radius))` }"
              >
                <component :is="feature.icon" class="w-4 h-4 text-white" />
              </div>
              <span class="text-sm text-white/90 font-medium">{{ feature.text }}</span>
            </div>
          </div>
        </div>

        <div class="flex items-center gap-6 text-white/50 text-sm animate-fade-in">
          <span>{{ t('login.copyright') }}</span>
          <span class="w-1 h-1 rounded-full bg-white/30" />
          <a href="#" class="hover:text-white/80 transition-colors">{{
            t('login.privacyPolicy')
          }}</a>
          <a href="#" class="hover:text-white/80 transition-colors">{{
            t('login.termsOfService')
          }}</a>
        </div>
      </div>
    </div>

    <div
      class="w-full lg:w-1/2 xl:w-[45%] flex items-center justify-center p-6 sm:p-8 lg:p-12 relative"
    >
      <div class="absolute top-6 right-6 z-20">
        <Button
          variant="ghost"
          size="icon"
          class="bg-background/80 backdrop-blur-sm border border-border/50 hover:bg-background transition-all duration-200 shadow-sm"
          :style="{ borderRadius: `calc(var(--radius) * 1.5)` }"
          :title="
            themeStore.themeMode === 'dark' ? t('login.switchToLight') : t('login.switchToDark')
          "
          @click="themeStore.toggleThemeMode()"
        >
          <Sun v-if="themeStore.themeMode === 'dark'" class="h-5 w-5 text-foreground" />
          <Moon v-else class="h-5 w-5 text-foreground" />
        </Button>
      </div>

      <div class="relative z-10 w-full max-w-[400px] space-y-6">
        <div class="lg:hidden flex flex-col items-center gap-3 mb-8 animate-fade-in">
          <div
            class="flex items-center justify-center w-14 h-14 bg-gradient-to-br from-primary to-primary/80 shadow-lg"
            :style="{ borderRadius: `calc(var(--radius) * 1.5)` }"
          >
            <span class="text-2xl font-bold text-primary-foreground">T</span>
          </div>
          <span class="text-xl font-bold text-foreground">MyBaseSystem</span>
        </div>

        <div class="space-y-2 animate-fade-in-up animation-delay-1">
          <h2 class="text-2xl font-bold text-foreground">
            {{ t('login.welcomeBack') }}
          </h2>
          <p class="text-muted-foreground">
            {{ t('login.loginSubtitle') }}
          </p>
        </div>

        <Transition
          enter-active-class="transition-all duration-300 ease-out"
          enter-from-class="opacity-0 -translate-y-2"
          enter-to-class="opacity-100 translate-y-0"
          leave-active-class="transition-all duration-200 ease-in"
          leave-from-class="opacity-100 translate-y-0"
          leave-to-class="opacity-0 -translate-y-2"
        >
          <div
            v-if="errorMessage"
            class="flex items-center gap-2 p-3 text-sm text-destructive bg-destructive/10 border border-destructive/20"
            :style="{ borderRadius: `calc(var(--radius))` }"
          >
            <div class="w-1.5 h-1.5 rounded-full bg-destructive flex-shrink-0" />
            {{ errorMessage }}
          </div>
        </Transition>

        <form class="space-y-4 animate-fade-in-up animation-delay-1" @submit.prevent="handleLogin">
          <div class="space-y-2">
            <Label for="email">{{ t('common.email') }}</Label>
            <div class="relative">
              <Mail
                class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground"
              />
              <Input
                id="email"
                v-model="email"
                type="text"
                placeholder="admin"
                required
                class="pl-10 h-11"
                autocomplete="username"
              />
            </div>
          </div>

          <div class="space-y-2">
            <Label for="password">{{ t('common.password') }}</Label>
            <div class="relative">
              <Lock
                class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground"
              />
              <Input
                id="password"
                v-model="password"
                :type="passwordType"
                placeholder="••••••••"
                required
                class="pl-10 pr-10 h-11"
                autocomplete="current-password"
              />
              <button
                type="button"
                class="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors"
                :aria-label="showPassword ? t('common.hidePassword') : t('common.showPassword')"
                @click="togglePasswordVisibility"
              >
                <Eye v-if="!showPassword" class="h-4 w-4" />
                <EyeOff v-else class="h-4 w-4" />
              </button>
            </div>
          </div>

          <div class="flex items-center justify-between animate-fade-in-up animation-delay-2">
            <div class="flex items-center space-x-2">
              <Checkbox id="remember" v-model:checked="rememberMe" :disabled="isLoading" />
              <label
                for="remember"
                class="text-sm text-muted-foreground cursor-pointer select-none hover:text-foreground transition-colors"
              >
                {{ t('common.rememberMe') }}
              </label>
            </div>
            <a
              href="#"
              class="text-sm text-muted-foreground hover:text-primary transition-colors"
              @click.prevent
            >
              {{ t('common.forgotPassword') }}
            </a>
          </div>

          <Button
            type="submit"
            class="w-full h-11 font-medium text-base transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md animate-fade-in-up animation-delay-2"
            :style="{ borderRadius: `calc(var(--radius))` }"
            :disabled="isLoading"
          >
            <span v-if="isLoading" class="flex items-center gap-2">
              <Loader2 class="animate-spin h-4 w-4" />
              {{ t('common.loggingIn') }}
            </span>
            <span v-else class="flex items-center gap-2">
              {{ t('common.login') }}
              <ArrowRight class="w-4 h-4" />
            </span>
          </Button>
        </form>

        <div
          class="p-4 bg-muted/50 border border-border/50 animate-fade-in-up animation-delay-3"
          :style="{ borderRadius: `calc(var(--radius) * 1.5)` }"
        >
          <div class="flex items-center gap-2 mb-2">
            <CheckCircle2 class="w-4 h-4 text-primary" />
            <span class="text-sm font-medium text-foreground">{{ t('common.demoAccount') }}</span>
          </div>
          <p class="text-xs text-muted-foreground leading-relaxed">
            {{ t('common.email') }}:
            <span class="text-foreground font-medium">admin</span>
            <br />
            {{ t('common.password') }}: <span class="text-foreground font-medium">Admin123!</span>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in {
  animation: fadeIn 0.6s ease-out forwards;
}

.animate-fade-in-up {
  opacity: 0;
  animation: fadeInUp 0.6s ease-out forwards;
}

.animation-delay-1 {
  animation-delay: 0.1s;
}
.animation-delay-2 {
  animation-delay: 0.2s;
}
.animation-delay-3 {
  animation-delay: 0.3s;
}

@keyframes pulse {
  0%,
  100% {
    opacity: 0.1;
  }
  50% {
    opacity: 0.15;
  }
}

.animate-pulse {
  animation: pulse 4s ease-in-out infinite;
}
</style>
