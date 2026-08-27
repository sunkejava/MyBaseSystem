<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { Button, Input } from '@tabtab/ui'
import { Plus, Search, Trash2, X } from 'lucide-vue-next'
import { systemApi, type Role, type UserListItem } from '@/api/client'

const rows = ref<UserListItem[]>([]); const roles = ref<Role[]>([]); const keyword = ref(''); const loading = ref(false); const showForm = ref(false); const error = ref('')
const form = reactive({ userName: '', displayName: '', email: '', phone: '', password: '', isEnabled: true, departmentId: null as string | null, roleIds: [] as string[] })
async function load(){loading.value=true;error.value='';try{const [users,roleList]=await Promise.all([systemApi.users(keyword.value),systemApi.roles()]);rows.value=users.items;roles.value=roleList}catch(e){error.value=e instanceof Error?e.message:'加载失败'}finally{loading.value=false}}
async function create(){error.value='';try{await systemApi.createUser(form);showForm.value=false;Object.assign(form,{userName:'',displayName:'',email:'',phone:'',password:'',isEnabled:true,departmentId:null,roleIds:[]});await load()}catch(e){error.value=e instanceof Error?e.message:'创建失败'}}
async function remove(row:UserListItem){if(!confirm(`确定删除用户 ${row.userName}？`))return;try{await systemApi.deleteUser(row.id);await load()}catch(e){error.value=e instanceof Error?e.message:'删除失败'}}
onMounted(load)
</script>

<template>
  <div class="space-y-5">
    <div class="flex flex-wrap items-center justify-between gap-3"><div><h1 class="text-3xl font-bold">用户管理</h1><p class="text-muted-foreground">维护账号、状态与角色绑定</p></div><Button @click="showForm=true"><Plus class="mr-2 h-4 w-4"/>新增用户</Button></div>
    <div class="flex gap-2"><Input v-model="keyword" placeholder="用户名、姓名或邮箱" class="max-w-sm" @keyup.enter="load"/><Button variant="outline" @click="load"><Search class="mr-2 h-4 w-4"/>查询</Button></div>
    <p v-if="error" class="rounded-md bg-destructive/10 p-3 text-sm text-destructive">{{ error }}</p>
    <div class="overflow-x-auto rounded-lg border"><table class="w-full text-sm"><thead class="bg-muted/50"><tr><th class="p-3 text-left">用户</th><th class="p-3 text-left">邮箱</th><th class="p-3 text-left">角色</th><th class="p-3">状态</th><th class="p-3 text-right">操作</th></tr></thead><tbody><tr v-for="row in rows" :key="row.id" class="border-t"><td class="p-3"><div class="font-medium">{{ row.displayName }}</div><div class="text-muted-foreground">{{ row.userName }}</div></td><td class="p-3">{{ row.email }}</td><td class="p-3">{{ row.roles.join('、') || '-' }}</td><td class="p-3 text-center"><span :class="row.isEnabled?'text-emerald-600':'text-muted-foreground'">{{ row.isEnabled?'启用':'停用' }}</span></td><td class="p-3 text-right"><Button size="icon" variant="ghost" :disabled="row.userName==='admin'" @click="remove(row)"><Trash2 class="h-4 w-4"/></Button></td></tr><tr v-if="!loading&&!rows.length"><td colspan="5" class="p-8 text-center text-muted-foreground">暂无数据</td></tr></tbody></table></div>
    <div v-if="showForm" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4"><form class="w-full max-w-lg space-y-4 rounded-xl bg-background p-6 shadow-xl" @submit.prevent="create"><div class="flex justify-between"><h2 class="text-xl font-semibold">新增用户</h2><Button type="button" size="icon" variant="ghost" @click="showForm=false"><X class="h-4 w-4"/></Button></div><div class="grid gap-3 sm:grid-cols-2"><Input v-model="form.userName" required placeholder="用户名"/><Input v-model="form.displayName" required placeholder="显示名称"/><Input v-model="form.email" required type="email" placeholder="邮箱"/><Input v-model="form.phone" placeholder="手机号"/><Input v-model="form.password" required type="password" placeholder="初始密码" class="sm:col-span-2"/></div><div><p class="mb-2 text-sm text-muted-foreground">角色</p><label v-for="role in roles" :key="role.id" class="mr-4 inline-flex gap-2"><input v-model="form.roleIds" type="checkbox" :value="role.id"/>{{ role.name }}</label></div><div class="flex justify-end gap-2"><Button type="button" variant="outline" @click="showForm=false">取消</Button><Button type="submit">保存</Button></div></form></div>
  </div>
</template>
