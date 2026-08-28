import type { RouteRecordRaw } from 'vue-router'
import { Building2,Menu,Settings } from 'lucide-vue-next'

const routes:RouteRecordRaw[]=[{
  path:'/system',name:'SystemManagement',redirect:'/system/menus',meta:{titleKey:'menu.systemManagement',title:'System Management',icon:Settings,order:3,permission:'system:menu:manage'},children:[
    {path:'/system/menus',name:'MenuManagement',component:()=>import('@/views/system/Menus.vue'),meta:{titleKey:'menu.menuManagement',title:'Menu Management',icon:Menu,order:1,keepAlive:true,permission:'system:menu:manage'}},
    {path:'/system/departments',name:'DepartmentManagement',component:()=>import('@/views/system/Departments.vue'),meta:{titleKey:'menu.departmentManagement',title:'Organization Management',icon:Building2,order:2,keepAlive:true,permission:'system:department:manage'}},
  ],
}]
export default routes
