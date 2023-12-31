﻿namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account.Manage;

using Microsoft.AspNetCore.Mvc.Rendering;

public static class ManageNavPages
{
	public static string Index => "Index";

	public static string Avatar => "Avatar";

	public static string Email => "Email";

	public static string ChangePassword => "ChangePassword";

	public static string DownloadPersonalData => "DownloadPersonalData";

	public static string DeletePersonalData => "DeletePersonalData";

	public static string PersonalData => "PersonalData";

	public static string IndexNavClass(ViewContext viewContext) =>
		PageNavClass(viewContext, Index);

	public static string AvatarNavClass(ViewContext viewContext) =>
		PageNavClass(viewContext, Avatar);

	public static string EmailNavClass(ViewContext viewContext) =>
		PageNavClass(viewContext, Email);

	public static string ChangePasswordNavClass(ViewContext viewContext) =>
		PageNavClass(viewContext, ChangePassword);

	public static string DownloadPersonalDataNavClass(ViewContext viewContext) =>
		PageNavClass(viewContext, DownloadPersonalData);

	public static string DeletePersonalDataNavClass(ViewContext viewContext) =>
		PageNavClass(viewContext, DeletePersonalData);

	public static string PersonalDataNavClass(ViewContext viewContext) =>
		PageNavClass(viewContext, PersonalData);

	public static string PageNavClass(ViewContext viewContext, string page)
	{
		string? activePage = viewContext.ViewData["ActivePage"] as string
			?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

		return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase)
			? "text-white bg-dark"
			: "text-info";
	}
}
