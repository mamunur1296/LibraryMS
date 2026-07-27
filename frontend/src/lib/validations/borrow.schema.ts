import { z } from "zod";

export const borrowBookSchema = z.object({
  memberId: z.string().min(1, "Member selection is required"),
  bookId: z.string().min(1, "Book selection is required"),
  bookCopyId: z.string().min(1, "Book Copy selection is required"),
  branchId: z.string().min(1, "Branch selection is required"),
  borrowDays: z.number().min(1, "Minimum borrow days is 1").max(30, "Maximum borrow days is 30").optional(),
});

export type BorrowBookFormData = z.infer<typeof borrowBookSchema>;

export const returnBookSchema = z.object({
  notes: z.string().optional(),
});

export type ReturnBookFormData = z.infer<typeof returnBookSchema>;
