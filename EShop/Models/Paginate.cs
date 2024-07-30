namespace EShop.Models
{
	public class Paginate
	{
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }

        public Paginate() 
        {
            
        }
        public Paginate(int totalItems, int page, int pageSize=10) //10 items/trang
        {
            //làm tròn tổng items/10 items trên 1 trang. VD: 16 items/10 = tròn 3 trang
            int totalPages = (int)Math.Ceiling((decimal)totalItems/(decimal)pageSize);

            int currentPage = page; //page hiện tại = 1

            int startPage = currentPage - 5; //trang bắt đầu trừ 5 button
            int endPage = currentPage + 4; //trang cuối sẽ cộng thêm 4 button

            if(startPage <= 0)
            {
                //nếu số trang nhỏ hơn hoặc bằng 0 thì số trang cuối sẽ bằng
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }
			if (endPage > totalPages) //nếu số page cuối > số tổng trang 
			{
				endPage = totalPages; //số page cuối = số tổng trang
				if (endPage > 10) //nếu số page cuối > 10
				{
					startPage = endPage - 9; //trang bắt đầu = trang cuối - 9
				}
			}
			TotalItems = totalItems;
			CurrentPage = currentPage;
			PageSize = pageSize;
			TotalPages = totalPages;
			StartPage = startPage;
			EndPage = endPage;
		}
	}
}
