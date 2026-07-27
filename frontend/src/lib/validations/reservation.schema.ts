import { z } from "zod";

export const createReservationSchema = z.object({
  memberId: z.string().min(1, "Member selection is required"),
  bookId: z.string().min(1, "Book selection is required"),
  branchId: z.string().min(1, "Branch selection is required"),
});

export type CreateReservationFormData = z.infer<typeof createReservationSchema>;
