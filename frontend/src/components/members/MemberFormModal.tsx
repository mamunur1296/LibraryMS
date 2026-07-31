import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { memberSchema, MemberFormData } from "@/lib/validations/member.schema";
import { memberService } from "@/lib/services/member.service";
import { MemberDto } from "@/types/member.types";
import { useEffect } from "react";

interface MemberFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  memberToEdit?: MemberDto | null;
}

export function MemberFormModal({ isOpen, onClose, onSuccess, memberToEdit }: MemberFormModalProps) {
  const {
    register,
    handleSubmit,
    reset,
    watch,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<MemberFormData>({
    resolver: zodResolver(memberSchema),
  });

  const createAccount = watch("createAccount");

  useEffect(() => {
    if (isOpen) {
      if (memberToEdit) {
        reset({
          firstName: memberToEdit.firstName,
          lastName: memberToEdit.lastName,
          email: memberToEdit.email,
          phone: memberToEdit.phone,
          address: memberToEdit.address || "",
          createAccount: false,
        });
      } else {
        reset({
          firstName: "",
          lastName: "",
          email: "",
          phone: "",
          address: "",
          createAccount: false,
          username: "",
          password: "",
        });
      }
    }
  }, [isOpen, memberToEdit, reset]);

  if (!isOpen) return null;

  const onSubmit = async (data: MemberFormData) => {
    try {
      if (memberToEdit) {
        await memberService.update(memberToEdit.id, data);
      } else {
        await memberService.create({
          ...data,
          username: data.createAccount ? data.username : undefined,
          password: data.createAccount ? data.password : undefined,
        });
      }
      reset();
      onSuccess();
      onClose();
    } catch (err: any) {
      const apiErrors = err.response?.data?.errors;
      if (apiErrors && typeof apiErrors === "object") {
        Object.keys(apiErrors).forEach((key) => {
          const fieldName = (key.charAt(0).toLowerCase() + key.slice(1)) as keyof MemberFormData;
          setError(fieldName, {
            type: "server",
            message: apiErrors[key][0],
          });
        });
      } else {
        alert(err.response?.data?.message || "Something went wrong.");
      }
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm overflow-y-auto py-10">
      <div className="w-full max-w-lg bg-slate-900 border border-slate-700 rounded-2xl shadow-2xl overflow-hidden m-4">
        <div className="flex items-center justify-between px-6 py-4 border-b border-slate-800 bg-slate-900/50 sticky top-0 z-10">
          <h2 className="text-xl font-semibold text-white">
            {memberToEdit ? "Edit Member" : "Add New Member"}
          </h2>
          <button onClick={onClose} className="text-slate-400 hover:text-white transition-colors">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="p-6">
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">First Name</label>
                <input
                  type="text"
                  {...register("firstName")}
                  className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
                {errors.firstName && <p className="text-red-400 text-xs mt-1">{errors.firstName.message}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">Last Name</label>
                <input
                  type="text"
                  {...register("lastName")}
                  className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
                {errors.lastName && <p className="text-red-400 text-xs mt-1">{errors.lastName.message}</p>}
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-slate-300 mb-1">Email</label>
              <input
                type="email"
                {...register("email")}
                disabled={!!memberToEdit}
                className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 disabled:opacity-50"
              />
              {errors.email && <p className="text-red-400 text-xs mt-1">{errors.email.message}</p>}
            </div>

            <div>
              <label className="block text-sm font-medium text-slate-300 mb-1">Phone</label>
              <input
                type="text"
                {...register("phone")}
                className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
              />
              {errors.phone && <p className="text-red-400 text-xs mt-1">{errors.phone.message}</p>}
            </div>

            <div>
              <label className="block text-sm font-medium text-slate-300 mb-1">Address</label>
              <textarea
                {...register("address")}
                rows={2}
                className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 resize-none"
              ></textarea>
              {errors.address && <p className="text-red-400 text-xs mt-1">{errors.address.message}</p>}
            </div>

            {!memberToEdit && (
              <div className="pt-4 border-t border-slate-800">
                <label className="flex items-center space-x-3 cursor-pointer group mb-4">
                  <div className="relative flex items-center justify-center">
                    <input
                      type="checkbox"
                      {...register("createAccount")}
                      className="peer sr-only"
                    />
                    <div className="w-5 h-5 border-2 border-slate-500 rounded bg-transparent peer-checked:bg-indigo-500 peer-checked:border-indigo-500 transition-all flex items-center justify-center">
                      <svg className="w-3.5 h-3.5 text-white opacity-0 peer-checked:opacity-100" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" />
                      </svg>
                    </div>
                  </div>
                  <span className="text-sm font-medium text-slate-300 group-hover:text-white transition-colors">
                    Create System Login Account
                  </span>
                </label>

                {createAccount && (
                  <div className="space-y-4 p-4 bg-slate-950/50 rounded-xl border border-indigo-500/20">
                    <div>
                      <label className="block text-sm font-medium text-slate-300 mb-1">Username</label>
                      <input
                        type="text"
                        {...register("username")}
                        className="w-full px-4 py-2 bg-slate-900 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                        placeholder="john.doe"
                      />
                      {errors.username && <p className="text-red-400 text-xs mt-1">{errors.username.message}</p>}
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-slate-300 mb-1">Password</label>
                      <input
                        type="password"
                        {...register("password")}
                        className="w-full px-4 py-2 bg-slate-900 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                        placeholder="••••••••"
                      />
                      {errors.password && <p className="text-red-400 text-xs mt-1">{errors.password.message}</p>}
                    </div>
                  </div>
                )}
              </div>
            )}

            <div className="flex justify-end space-x-3 pt-4 border-t border-slate-800">
              <button
                type="button"
                onClick={onClose}
                className="px-4 py-2 rounded-lg text-sm font-medium text-slate-300 hover:text-white hover:bg-slate-800 transition-colors"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={isSubmitting}
                className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg text-sm font-medium transition-colors shadow-lg shadow-indigo-500/20 disabled:opacity-50 flex items-center"
              >
                {isSubmitting ? (
                  <span className="mr-2 border-2 border-white/20 border-t-white rounded-full w-4 h-4 animate-spin"></span>
                ) : null}
                {memberToEdit ? "Update Member" : "Add Member"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
