import { z } from "zod";

export const authorSchema = z.object({
  name: z.string().min(2, "Author name must be at least 2 characters").max(100, "Author name is too long"),
  biography: z.string().optional(),
});

export type AuthorFormData = z.infer<typeof authorSchema>;

export const categorySchema = z.object({
  name: z.string().min(2, "Category name must be at least 2 characters").max(100, "Category name is too long"),
  description: z.string().optional(),
});

export type CategoryFormData = z.infer<typeof categorySchema>;

export const bookSchema = z.object({
  title: z.string().min(2, "Title is required"),
  isbn: z.string().min(1, "ISBN is required").min(10, "ISBN must be at least 10 characters").max(20, "ISBN is too long"),
  description: z.string().optional(),
  publicationYear: z.number().min(1000, "Invalid year").max(new Date().getFullYear(), "Year cannot be in the future"),
  categoryId: z.string().min(1, "Category is required"),
  authorId: z.string().min(1, "Author is required"),
  language: z.string().min(2, "Language is required"),
  
  // Fields for creation only (not update)
  initialCopies: z.number().min(1, "Must have at least 1 copy").optional(),
  branchId: z.string().optional(),
});

export type BookFormData = z.infer<typeof bookSchema>;
