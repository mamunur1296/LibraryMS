import { apiClient } from "@/lib/api-client";
import {
  BookDto, PagedResult, AuthorDto, CategoryDto,
  CreateBookRequest, UpdateBookRequest, CreateAuthorRequest, CreateCategoryRequest,
  BookCopy
} from "@/types/book.types";

export const bookService = {
  // Books
  async search(
    searchTerm?: string, 
    categoryId?: string, 
    authorId?: string, 
    branchId?: string, 
    page: number = 1, 
    pageSize: number = 10
  ): Promise<PagedResult<BookDto>> {
    const params = new URLSearchParams();
    if (searchTerm) params.append("searchTerm", searchTerm);
    if (categoryId) params.append("categoryId", categoryId);
    if (authorId) params.append("authorId", authorId);
    if (branchId) params.append("branchId", branchId);
    params.append("page", page.toString());
    params.append("pageSize", pageSize.toString());

    const response = await apiClient.get<PagedResult<BookDto>>(`/api/Books?${params.toString()}`);
    return response.data;
  },

  async getById(id: string): Promise<BookDto> {
    const response = await apiClient.get<BookDto>(`/api/Books/${id}`);
    return response.data;
  },

  async create(data: CreateBookRequest): Promise<BookDto> {
    const response = await apiClient.post<BookDto>("/api/Books", data);
    return response.data;
  },

  async update(id: string, data: Omit<UpdateBookRequest, "id">): Promise<BookDto> {
    const response = await apiClient.put<BookDto>(`/api/Books/${id}`, { id, ...data });
    return response.data;
  },

  async addCopies(id: string, data: { branchId: string; quantity: number }): Promise<any[]> {
    const response = await apiClient.post<any[]>(`/api/Books/${id}/copies`, { bookId: id, ...data });
    return response.data;
  },

  async delete(id: string): Promise<void> {
    await apiClient.delete(`/api/Books/${id}`);
  },

  // Authors
  async getAllAuthors(): Promise<AuthorDto[]> {
    const response = await apiClient.get<AuthorDto[]>("/api/Books/authors");
    return response.data;
  },

  async createAuthor(data: CreateAuthorRequest): Promise<AuthorDto> {
    const response = await apiClient.post<AuthorDto>("/api/Books/authors", data);
    return response.data;
  },

  // Categories
  async getAllCategories(): Promise<CategoryDto[]> {
    const response = await apiClient.get<CategoryDto[]>("/api/Books/categories");
    return response.data;
  },

  async createCategory(data: CreateCategoryRequest): Promise<CategoryDto> {
    const response = await apiClient.post<CategoryDto>("/api/Books/categories", data);
    return response.data;
  },

  async getBookCopies(id: string): Promise<BookCopy[]> {
    const { data } = await apiClient.get<BookCopy[]>(`/api/Books/${id}/copies`);
    return data;
  }
};
