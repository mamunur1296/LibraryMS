export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface BookDto {
  id: string;
  title: string;
  isbn: string;
  description: string | null;
  publicationYear: number;
  language: string;
  coverImageUrl: string | null;
  categoryName: string;
  categoryId: string;
  authorName: string;
  authorId: string;
  totalCopies: number;
  availableCopies: number;
  createdAt: string;
}

export interface AuthorDto {
  id: string;
  name: string;
  biography: string | null;
}

export interface CategoryDto {
  id: string;
  name: string;
  description: string | null;
}

export interface CreateBookRequest {
  title: string;
  isbn: string;
  description?: string;
  publicationYear: number;
  categoryId: string;
  authorId: string;
  language: string;
  initialCopies: number;
  branchId: string;
}

export interface UpdateBookRequest {
  id: string;
  title: string;
  description?: string;
  publicationYear: number;
  categoryId: string;
  authorId: string;
  language: string;
}

export interface CreateAuthorRequest {
  name: string;
  biography?: string;
}

export interface CreateCategoryRequest {
  name: string;
  description?: string;
}

export interface AddBookCopiesRequest {
  bookId: string;
  branchId: string;
  quantity: number;
}
