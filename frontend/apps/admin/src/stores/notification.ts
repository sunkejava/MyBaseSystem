import { computed,ref } from 'vue'
import { defineStore } from 'pinia'
import { systemApi,type NotificationItem } from '@/api/client'

export type NotificationType='info'|'success'|'warning'|'error'|'message'
export interface Notification { id:string;title:string;message:string;type:NotificationType;read:boolean;createdAt:Date;actionUrl?:string;actionLabel?:string }

/** 真实站内通知状态：所有变更均同步到服务端。 */
export const useNotificationStore=defineStore('notification',()=>{
  const notifications=ref<Notification[]>([]),loading=ref(false)
  const unreadCount=computed(()=>notifications.value.filter(x=>!x.read).length)
  const notificationList=computed(()=>[...notifications.value].sort((a,b)=>b.createdAt.getTime()-a.createdAt.getTime()))
  const typeCount=computed(()=>notifications.value.reduce<Record<NotificationType,number>>((r,x)=>(r[x.type]++,r),{info:0,success:0,warning:0,error:0,message:0}))
  const map=(x:NotificationItem):Notification=>({id:x.id,title:x.title,message:x.message,type:x.type,read:x.isRead,createdAt:new Date(x.createdAt),actionUrl:x.actionUrl})
  async function load(){loading.value=true;try{notifications.value=(await systemApi.notifications()).map(map)}finally{loading.value=false}}
  async function markAsRead(id:string){await systemApi.markNotificationRead(id);const item=notifications.value.find(x=>x.id===id);if(item)item.read=true}
  async function markAllAsRead(){await systemApi.markAllNotificationsRead();notifications.value.forEach(x=>x.read=true)}
  async function removeNotification(id:string){await systemApi.deleteNotification(id);notifications.value=notifications.value.filter(x=>x.id!==id)}
  async function clearAll(){for(const item of [...notifications.value])await removeNotification(item.id)}
  return{notifications,loading,unreadCount,notificationList,typeCount,load,markAsRead,markAllAsRead,removeNotification,clearAll}
})
