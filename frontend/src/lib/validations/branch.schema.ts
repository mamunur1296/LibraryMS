import { z } from "zod";

export const branchSchema = z.object({
  name: z.string().min(2, "Name must be at least 2 characters").max(100, "Name is too long"),
  address: z.string().min(5, "Address must be at least 5 characters"),
  phone: z.string().min(5, "Phone number is required"),
  email: z.string().email("Invalid email address"),
});

export type BranchFormData = z.infer<typeof branchSchema>;
