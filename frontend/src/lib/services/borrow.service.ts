import { apiClient } from "../api-client";
import { BorrowDto, BorrowBookRequest, ReturnBookRequest, PagedResult } from "../../types/borrow.types";

export const borrowService = {
  async search(
    memberId?: string, 
    bookId?: string, 
    status?: string, 
    page: number = 1, 
    pageSize: number = 10
  ): Promise<PagedResult<BorrowDto>> {
    const params = new URLSearchParams();
    if (memberId) params.append("memberId", memberId);
    if (bookId) params.append("bookId", bookId);
    if (status) params.append("status", status);
    params.append("page", page.toString());
    params.append("pageSize", pageSize.toString());

    const response = await apiClient.get<PagedResult<BorrowDto>>(`/api/Borrows?${params.toString()}`);
    return response.data;
  },

  async borrowBook(data: BorrowBookRequest): Promise<BorrowDto> {
    const response = await apiClient.post<BorrowDto>("/api/Borrows", data);
    return response.data;
  },

  async returnBook(data: ReturnBookRequest): Promise<BorrowDto> {
    const response = await apiClient.post<BorrowDto>("/api/Borrows/return", data);
    return response.data;
  },

  async payFine(borrowId: string): Promise<BorrowDto> {
    const response = await apiClient.post<BorrowDto>(`/api/Borrows/${borrowId}/pay-fine`);
    return response.data;
  }
};
