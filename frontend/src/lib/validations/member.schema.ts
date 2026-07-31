import { z } from "zod";

export const memberSchema = z.object({
  firstName: z.string().min(2, "First name must be at least 2 characters"),
  lastName: z.string().min(2, "Last name must be at least 2 characters"),
  email: z.string().email("Invalid email address"),
  phone: z.string().min(5, "Phone number is required"),
  address: z.string().optional(),
  
  createAccount: z.boolean().optional(),
  username: z.string().min(3, "Username must be at least 3 characters").optional().or(z.literal('')),
  password: z.string().min(6, "Password must be at least 6 characters").optional().or(z.literal('')),
}).superRefine((data, ctx) => {
  if (data.createAccount) {
    if (!data.username || data.username.trim() === "") {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Username is required when creating an account",
        path: ["username"],
      });
    }
    if (!data.password || data.password.trim() === "") {
      ctx.addIssue({
        code: z.ZodIssueCode.custom,
        message: "Password is required when creating an account",
        path: ["password"],
      });
    }
  }
});

export type MemberFormData = z.infer<typeof memberSchema>;

export const suspendMemberSchema = z.object({
  suspendedUntil: z.string().min(1, "Suspension end date is required"),
  reason: z.string().min(5, "Reason must be at least 5 characters"),
});

export type SuspendMemberFormData = z.infer<typeof suspendMemberSchema>;

export const resetPasswordSchema = z.object({
  newPassword: z.string().min(6, "Password must be at least 6 characters"),
  confirmPassword: z.string().min(6, "Confirm password must be at least 6 characters"),
}).refine((data) => data.newPassword === data.confirmPassword, {
  message: "Passwords do not match",
  path: ["confirmPassword"],
});

export type ResetPasswordFormData = z.infer<typeof resetPasswordSchema>;
