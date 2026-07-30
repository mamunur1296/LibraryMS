import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { resetPasswordSchema, ResetPasswordFormData } from "@/lib/validations/member.schema";
import { memberService } from "@/lib/services/member.service";
import { MemberDto } from "@/types/member.types";
import { toast } from "@/components/ui/Toast";

const createAccountSchema = z.object({
  username: z.string().min(3, "Username must be at least 3 characters"),
  password: z.string().min(6, "Password must be at least 6 characters"),
  confirmPassword: z.string().min(6, "Confirm password must be at least 6 characters"),
}).refine((data) => data.password === data.confirmPassword, {
  message: "Passwords do not match",
  path: ["confirmPassword"],
});

type CreateAccountFormData = z.infer<typeof createAccountSchema>;

interface ResetPasswordModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess?: () => void;
  member: MemberDto | null;
}

export function ResetPasswordModal({ isOpen, onClose, onSuccess, member }: ResetPasswordModalProps) {
  const hasAccount = member?.hasAccount ?? false;

  // Form for resetting password
  const resetForm = useForm<ResetPasswordFormData>({
    resolver: zodResolver(resetPasswordSchema),
    defaultValues: {
      newPassword: "",
      confirmPassword: ""
    }
  });

  // Form for creating account
  const createForm = useForm<CreateAccountFormData>({
    resolver: zodResolver(createAccountSchema),
    defaultValues: {
      username: "",
      password: "",
      confirmPassword: ""
    }
  });

  if (!isOpen || !member) return null;

  const onResetSubmit = async (data: ResetPasswordFormData) => {
    try {
      await memberService.resetPassword(member.id, { newPassword: data.newPassword });
      toast.success(`Password reset successfully for ${member.fullName}`);
      resetForm.reset();
      onClose();
      if (onSuccess) onSuccess();
    } catch (err: any) {
      toast.error(err.response?.data?.message || "Failed to reset password");
    }
  };

  const onCreateSubmit = async (data: CreateAccountFormData) => {
    try {
      await memberService.createAccount(member.id, {
        username: data.username,
        password: data.password
      });
      toast.success(`Login account created successfully for ${member.fullName}`);
      createForm.reset();
      onClose();
      if (onSuccess) onSuccess();
    } catch (err: any) {
      toast.error(err.response?.data?.message || "Failed to create account");
    }
  };

  return (
    <div className="fixed inset-0 z-[60] flex items-center justify-center bg-black/60 backdrop-blur-sm">
      <div className="w-full max-w-sm bg-slate-800 border border-slate-700 rounded-xl shadow-2xl overflow-hidden">
        <div className="flex items-center justify-between px-5 py-3 border-b border-slate-700 bg-slate-800">
          <h3 className="text-lg font-medium text-white">
            {hasAccount ? "Reset Password" : "Create Login Account"}
          </h3>
          <button
            type="button"
            onClick={() => {
              resetForm.reset();
              createForm.reset();
              onClose();
            }}
            className="text-slate-400 hover:text-white transition-colors"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="p-5">
          <p className="text-xs text-slate-400 mb-4">
            {hasAccount
              ? `Reset password for account associated with ${member.fullName} (${member.email}).`
              : `Create a new login user account for ${member.fullName} (${member.email}).`}
          </p>

          {hasAccount ? (
            <form onSubmit={resetForm.handleSubmit(onResetSubmit)} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">New Password</label>
                <input
                  type="password"
                  {...resetForm.register("newPassword")}
                  className="w-full px-3 py-2 bg-slate-900 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 text-sm"
                  placeholder="Enter at least 6 characters"
                />
                {resetForm.formState.errors.newPassword && (
                  <p className="text-red-400 text-xs mt-1">{resetForm.formState.errors.newPassword.message}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">Confirm New Password</label>
                <input
                  type="password"
                  {...resetForm.register("confirmPassword")}
                  className="w-full px-3 py-2 bg-slate-900 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 text-sm"
                  placeholder="Re-enter password"
                />
                {resetForm.formState.errors.confirmPassword && (
                  <p className="text-red-400 text-xs mt-1">{resetForm.formState.errors.confirmPassword.message}</p>
                )}
              </div>

              <div className="flex justify-end space-x-3 pt-2">
                <button
                  type="button"
                  onClick={() => {
                    resetForm.reset();
                    onClose();
                  }}
                  className="px-3 py-1.5 rounded-lg text-sm font-medium text-slate-300 hover:text-white hover:bg-slate-700 transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={resetForm.formState.isSubmitting}
                  className="px-3 py-1.5 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg text-sm font-medium transition-colors shadow-lg shadow-indigo-500/20 disabled:opacity-50 flex items-center"
                >
                  {resetForm.formState.isSubmitting && (
                    <span className="mr-2 border-2 border-white/20 border-t-white rounded-full w-3 h-3 animate-spin"></span>
                  )}
                  Reset Password
                </button>
              </div>
            </form>
          ) : (
            <form onSubmit={createForm.handleSubmit(onCreateSubmit)} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">Username</label>
                <input
                  type="text"
                  {...createForm.register("username")}
                  className="w-full px-3 py-2 bg-slate-900 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 text-sm"
                  placeholder="Enter username (min 3 chars)"
                />
                {createForm.formState.errors.username && (
                  <p className="text-red-400 text-xs mt-1">{createForm.formState.errors.username.message}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">Password</label>
                <input
                  type="password"
                  {...createForm.register("password")}
                  className="w-full px-3 py-2 bg-slate-900 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 text-sm"
                  placeholder="Enter password (min 6 chars)"
                />
                {createForm.formState.errors.password && (
                  <p className="text-red-400 text-xs mt-1">{createForm.formState.errors.password.message}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">Confirm Password</label>
                <input
                  type="password"
                  {...createForm.register("confirmPassword")}
                  className="w-full px-3 py-2 bg-slate-900 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 text-sm"
                  placeholder="Re-enter password"
                />
                {createForm.formState.errors.confirmPassword && (
                  <p className="text-red-400 text-xs mt-1">{createForm.formState.errors.confirmPassword.message}</p>
                )}
              </div>

              <div className="flex justify-end space-x-3 pt-2">
                <button
                  type="button"
                  onClick={() => {
                    createForm.reset();
                    onClose();
                  }}
                  className="px-3 py-1.5 rounded-lg text-sm font-medium text-slate-300 hover:text-white hover:bg-slate-700 transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={createForm.formState.isSubmitting}
                  className="px-3 py-1.5 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg text-sm font-medium transition-colors shadow-lg shadow-indigo-500/20 disabled:opacity-50 flex items-center"
                >
                  {createForm.formState.isSubmitting && (
                    <span className="mr-2 border-2 border-white/20 border-t-white rounded-full w-3 h-3 animate-spin"></span>
                  )}
                  Create Account
                </button>
              </div>
            </form>
          )}
        </div>
      </div>
    </div>
  );
}
