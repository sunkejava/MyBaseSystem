using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBaseSystem.Application;

namespace MyBaseSystem.Api.Controllers;

/// <summary>提供仪表盘、站内通知和系统设置接口。</summary>
[ApiController,Authorize,Route("api/v1")]
public sealed class OperationsController(IOperationsApplicationService service) : ControllerBase
{
    private Guid UserId=>Guid.Parse(User.FindFirst("sub")!.Value);
    [HttpGet("dashboard/summary")] public async Task<ApiResult<DashboardSummary>> Summary(CancellationToken ct)=>ApiResult<DashboardSummary>.Ok(await service.SummaryAsync(UserId,ct));
    [HttpGet("notifications")] public async Task<ApiResult<IReadOnlyList<NotificationDto>>> Notifications(CancellationToken ct)=>ApiResult<IReadOnlyList<NotificationDto>>.Ok(await service.NotificationsAsync(UserId,ct));
    [HttpPost("notifications"),Authorize(Policy="admin")] public async Task<ApiResult<Guid>> CreateNotification(CreateNotificationRequest request,CancellationToken ct)=>ApiResult<Guid>.Ok(await service.CreateNotificationAsync(request,ct));
    [HttpPut("notifications/{id:guid}/read")] public async Task<ApiResult<object>> MarkRead(Guid id,CancellationToken ct){await service.MarkReadAsync(id,UserId,ct);return ApiResult<object>.Ok(new{});}
    [HttpPut("notifications/read-all")] public async Task<ApiResult<object>> MarkAllRead(CancellationToken ct){await service.MarkAllReadAsync(UserId,ct);return ApiResult<object>.Ok(new{});}
    [HttpDelete("notifications/{id:guid}")] public async Task<ApiResult<object>> DeleteNotification(Guid id,CancellationToken ct){await service.DeleteNotificationAsync(id,UserId,ct);return ApiResult<object>.Ok(new{});}
    [HttpGet("settings"),Authorize(Policy="admin")] public async Task<ApiResult<IReadOnlyList<SettingDto>>> Settings(CancellationToken ct)=>ApiResult<IReadOnlyList<SettingDto>>.Ok(await service.SettingsAsync(ct));
    [HttpPut("settings/{key}"),Authorize(Policy="admin")] public async Task<ApiResult<object>> SaveSetting(string key,SaveSettingRequest request,CancellationToken ct){await service.SaveSettingAsync(key,request,ct);return ApiResult<object>.Ok(new{});}
}
