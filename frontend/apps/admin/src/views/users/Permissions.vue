<script setup lang="ts">
import { computed,onMounted,ref } from 'vue'
import { Badge,Button,Card,CardContent,Input,Table,TableBody,TableCell,TableEmpty,TableHead,TableHeader,TableRow } from '@tabtab/ui'
import { RotateCcw,Search } from 'lucide-vue-next'
import { systemApi,type Permission } from '@/api/client'
const rows=ref<Permission[]>([]),keyword=ref(''),group=ref(''),loading=ref(false),error=ref('')
const groups=computed(()=>[...new Set(rows.value.map(x=>x.group))])
const filtered=computed(()=>rows.value.filter(x=>(!keyword.value||`${x.code}${x.name}`.toLowerCase().includes(keyword.value.toLowerCase()))&&(!group.value||x.group===group.value)))
async function load(){loading.value=true;error.value='';try{rows.value=await systemApi.permissions()}catch(e){error.value=e instanceof Error?e.message:'加载失败'}finally{loading.value=false}}
function reset(){keyword.value='';group.value=''}
onMounted(load)
</script>
<template><div class="space-y-5"><div><h1 class="text-3xl font-bold">权限管理</h1><p class="text-muted-foreground">查看系统权限点及所属业务分组，角色授权时直接使用这些权限标识。</p></div><Card><CardContent class="pt-6"><div class="grid gap-3 md:grid-cols-[1fr_220px_auto]"><Input v-model="keyword" placeholder="权限名称或编码"/><select v-model="group" class="h-10 rounded-md border bg-background px-3"><option value="">全部分组</option><option v-for="item in groups" :key="item">{{item}}</option></select><div class="flex gap-2"><Button><Search class="mr-2 h-4 w-4"/>查询</Button><Button variant="outline" @click="reset"><RotateCcw class="mr-2 h-4 w-4"/>重置</Button></div></div></CardContent></Card><p v-if="error" class="text-sm text-destructive">{{error}}</p><Card><CardContent class="p-0"><Table><TableHeader><TableRow><TableHead>权限名称</TableHead><TableHead>权限编码</TableHead><TableHead>分组</TableHead><TableHead>说明</TableHead></TableRow></TableHeader><TableBody><TableRow v-for="row in filtered" :key="row.id"><TableCell class="font-medium">{{row.name}}</TableCell><TableCell class="font-mono text-xs">{{row.code}}</TableCell><TableCell><Badge variant="secondary">{{row.group}}</Badge></TableCell><TableCell class="text-muted-foreground">{{row.description||'-'}}</TableCell></TableRow><TableEmpty v-if="!loading&&!filtered.length" :colspan="4">暂无符合条件的权限</TableEmpty></TableBody></Table></CardContent></Card></div></template>
