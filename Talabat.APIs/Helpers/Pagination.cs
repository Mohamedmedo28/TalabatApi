using Talabat.APIs.Dtos;

namespace Talabat.APIs.Helpers
{
    public class Pagination<T>//standard Response any paginations
    {
      

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int Count { get; set; }//all elements

        public IReadOnlyList<T> Data { get; set; }

        public Pagination(int pageSize, int pageIndex, IReadOnlyList<T> data ,int count)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
          Data= data;
            Count = count;
        }

      
    }
}
