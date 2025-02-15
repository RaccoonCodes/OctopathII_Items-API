using Microsoft.EntityFrameworkCore;
using OctopathII_Items.DTO;
using System.Linq.Dynamic.Core;

namespace OctopathII_Items.Extensions
{
    public static class GenericPaginatedData
    {
        public static async Task<RestDTO<T[]>> GetPaginatedDataAsync<T>(
            RequestDTO<T> requestDTO, IQueryable<T> query,
            string base_url, string rel, string action) where T : class
        {
            if (requestDTO.PageIndex < 0 || requestDTO.PageSize <= 0)
            {
                return new RestDTO<T[]>()
                {
                    Data = []
                };
            }

            // Apply filtering
            if (!string.IsNullOrEmpty(requestDTO.FilterQuery))
            {
                if (!string.IsNullOrEmpty(requestDTO.SortColumn))
                {
                    query = query.Where($"{requestDTO.SortColumn}.Contains(@0)", requestDTO.FilterQuery);
                }
                else if (typeof(T).GetProperty("Name") != null)
                {
                    query = query.Where("Name.Contains(@0)", requestDTO.FilterQuery);
                }
            }

            var recordCount = await query.CountAsync();

            if (recordCount == 0)
            {
                return new RestDTO<T[]>
                {
                    Data = Array.Empty<T>(),
                    PageIndex = requestDTO.PageIndex,
                    PageSize = requestDTO.PageSize,
                    RecordCount = recordCount,
                    Message = "No Records found for this input",
                    Links = new List<LinkDTO>()
                };
            }

            var totalPages = (int)Math.Ceiling(recordCount / (double)requestDTO.PageSize);

            T[]? result = await query.OrderBy($"{requestDTO.SortColumn} {requestDTO.SortOrder}")
                            .Skip(requestDTO.PageIndex * requestDTO.PageSize)
                            .Take(requestDTO.PageSize)
                            .ToArrayAsync();

            var links = PaginationHelper.GeneratePaginationLinks(base_url, rel, action,
                requestDTO.PageIndex, requestDTO.PageSize, totalPages,
                new Dictionary<string, string> {
                    { "SortColumn", requestDTO.SortColumn ?? string.Empty },
                    { "SortOrder",  requestDTO.SortOrder ?? string.Empty },
                    { "FilterQuery", requestDTO.FilterQuery ?? string.Empty }
                }
            );

            return new RestDTO<T[]>
            {
                Data = result,
                PageIndex = requestDTO.PageIndex,
                PageSize = requestDTO.PageSize,
                RecordCount = recordCount,
                TotalPages = totalPages,
                Message = "Successful retrieval",
                Links = links
            };
        }
    }
}
