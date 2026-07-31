import { apiClient } from '../api-client';
import { BookDto } from '../../types/book.types';

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
}

export interface GetBooksParams {
  searchTerm?: string;
  categoryId?: string;
  authorId?: string;
  branchId?: string;
  page?: number;
  pageSize?: number;
}

export const publicService = {
  getBooks: async (params: GetBooksParams): Promise<PagedResult<BookDto>> => {
    const response = await apiClient.get<PagedResult<BookDto>>('/api/public/books', { params });
    return response.data;
  },

  getBookById: async (id: string): Promise<BookDto> => {
    const response = await apiClient.get<BookDto>(`/api/public/books/${id}`);
    return response.data;
  },
};
