* ta sẽ sử dụng **`AuthorizationService.AuthorizeAsync`** để ẩn hiện các nút chức năng - việc này nhằm user-friendly chứ không phải security gì cả
* _ở đây cả user thường, manager, và admin đều có thể vô "/Detail" page nhưng sẽ có các nút chức năng khác nhau_


============================================================================
# Inject the authorization service into the views
* -> _currently, the UI shows edit and delete links for contacts the user can't modify_
* -> **`inject the authorization service`** in the **Pages/_ViewImports.cshtml** file so it's **`available to all views`**:

```cs
@using Microsoft.AspNetCore.Identity
@using ContactManager
@using ContactManager.Data
@namespace ContactManager.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@using ContactManager.Authorization; // this one
@using Microsoft.AspNetCore.Authorization // this one
@using ContactManager.Models // this one
@inject IAuthorizationService AuthorizationService // this one
```

============================================================================
# Update Contacts page
* -> update the **Edit** and **Delete** links in _Pages/Contacts/Index.cshtml_ so they're **`only rendered for users with the appropriate permissions`**:
* -> _remember `hiding links` from users that don't have permission to change data **doesn't secure the app**. Hiding links makes the app more **user-friendly** by displaying only valid links_

```cs
@page
@model ContactManager.Pages.Contacts.IndexModel

@{
    ViewData["Title"] = "Index";
}

<h1>Index</h1>

<p>
    <a asp-page="Create">Create New</a>
</p>
<table class="table">
    <thead>
        <tr>
            <th>
                @Html.DisplayNameFor(model => model.Contact[0].Name)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Contact[0].Address)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Contact[0].City)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Contact[0].State)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Contact[0].Zip)
            </th>
            <th>
                @Html.DisplayNameFor(model => model.Contact[0].Email)
            </th>
             <th> // this one
                @Html.DisplayNameFor(model => model.Contact[0].Status)
            </th>
            <th></th>
        </tr>
    </thead>
    <tbody>
@foreach (var item in Model.Contact) {
        <tr>
            <td>
                @Html.DisplayFor(modelItem => item.Name)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Address)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.City)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.State)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Zip)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Email)
            </td>
                <td> // this one
                    @Html.DisplayFor(modelItem => item.Status)
                </td>
                <td> // this one
                    @if ((await AuthorizationService
                        .AuthorizeAsync(User, item, ContactOperations.Update)).Succeeded)
                    {
                        <a asp-page="./Edit" asp-route-id="@item.ContactId">Edit</a>
                        <text> | </text>
                    }

                    <a asp-page="./Details" asp-route-id="@item.ContactId">Details</a>

                    @if ((await AuthorizationService
                        .AuthorizeAsync(User, item,ContactOperations.Delete)).Succeeded)
                    {
                        <text> | </text>
                        <a asp-page="./Delete" asp-route-id="@item.ContactId">Delete</a>
                    }
                </td>
            </tr>
        }
    </tbody>
</table>
```

============================================================================
## Update Details
* -> update the details view so **`managers can approve or reject contacts`**:

```cs
@*Preceding markup omitted for brevity.*@
        <dt class="col-sm-2">
            @Html.DisplayNameFor(model => model.Contact.Email)
        </dt>
        <dd class="col-sm-10">
            @Html.DisplayFor(model => model.Contact.Email)
        </dd>
    <dt>
            @Html.DisplayNameFor(model => model.Contact.Status)
        </dt>
        <dd>
            @Html.DisplayFor(model => model.Contact.Status)
        </dd>
    </dl>
</div>

@if (Model.Contact.Status != ContactStatus.Approved)
{
    @if ((await AuthorizationService.AuthorizeAsync(
     User, Model.Contact, ContactOperations.Approve)).Succeeded)
    {
        <form style="display:inline;" method="post">
            <input type="hidden" name="id" value="@Model.Contact.ContactId" />
            <input type="hidden" name="status" value="@ContactStatus.Approved" />
            <button type="submit" class="btn btn-xs btn-success">Approve</button>
        </form>
    }
}

@if (Model.Contact.Status != ContactStatus.Rejected)
{
    @if ((await AuthorizationService.AuthorizeAsync(
     User, Model.Contact, ContactOperations.Reject)).Succeeded)
    {
        <form style="display:inline;" method="post">
            <input type="hidden" name="id" value="@Model.Contact.ContactId" />
            <input type="hidden" name="status" value="@ContactStatus.Rejected" />
            <button type="submit" class="btn btn-xs btn-danger">Reject</button>
        </form>
    }
}

<div>
    @if ((await AuthorizationService.AuthorizeAsync(
         User, Model.Contact,
         ContactOperations.Update)).Succeeded)
    {
        <a asp-page="./Edit" asp-route-id="@Model.Contact.ContactId">Edit</a>
        <text> | </text>
    }
    <a asp-page="./Index">Back to List</a>
</div>
```
