USE [Tabi]
GO
/****** Object:  StoredProcedure [dbo].[Venues_SelectAll]    Script Date: 4/25/2024 5:19:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- ==================================
--	Author: CJ Clifton
--	Create date: 04/04/2024
--	Description: Used to select all venues and return a paginated response
--	Code Reviewer: Blake R

--	Modified by: N/A
--	Modified date: N/A
--	Code Reviewer: N/A
--	Note: N/A

--	Modified by: CJ Clifton
--	Modified date: 04/24/2024
--	Code Reviewer: Byron
--	Note: Added new select subquery for locations
-- ===================================


CREATE proc [dbo].[Venues_SelectAll]
					@PageIndex int
					,@PageSize int
as


/* TEST CODE -----------------------

Execute dbo.Venues_SelectAll @PageIndex=0, @PageSize = 20

*/


BEGIN

		Declare @offset int = @PageIndex * @PageSize

		SELECT V.[Id]
			  ,V.[Name]
			  ,V.[Description]

			  ,(select L.Id
						,L.LineOne
						,L.LineTwo
						,L.City
						,L.Zip
						,S.Id as StateId
						,S.[Name] as [State]
				From dbo.Locations as L
				Where L.Id = V.LocationId
				 FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) as LocationInfo

			  ,VT.Id as VenueTypeId
			  ,VT.[Name] as VenueType
			  ,F.[Url] as FileImageUrl
			  ,V.[Url]
			  ,dbo.fn_GetUserJSON(V.Createdby) as CreatedBy
			  ,dbo.fn_GetUserJSON(V.ModifiedBy) as ModifiedBy
			  ,V.[DateCreated]
			  ,V.[DateModified]
			  ,TotalCount = COUNT(1) OVER()
		FROM [dbo].[Venues] as V
		inner join dbo.VenueTypes as VT
		on V.VenueTypeId= VT.Id
		inner join dbo.Files as F
		on V.FileId = F.Id
		inner join dbo.Locations as L
		on L.Id = V.LocationId
		inner join dbo.States as S
		on S.Id = L.StateId

		ORDER BY V.Id
		OFFSET @offSet Rows
		Fetch Next @PageSize Rows ONLY


END
GO
