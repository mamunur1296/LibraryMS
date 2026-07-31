import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { branchSchema, BranchFormData } from "@/lib/validations/branch.schema";
import { branchService } from "@/lib/services/branch.service";
import { BranchDto } from "@/types/branch.types";
import { useEffect } from "react";
import { toast } from "@/components/ui/Toast";

interface BranchFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  branchToEdit?: BranchDto | null;
}

export function BranchFormModal({ isOpen, onClose, onSuccess, branchToEdit }: BranchFormModalProps) {
  const {
    register,
    handleSubmit,
    reset,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<BranchFormData>({
    resolver: zodResolver(branchSchema),
    defaultValues: {
      name: "",
      address: "",
      phone: "",
      email: "",
    },
  });

  useEffect(() => {
    if (branchToEdit) {
      reset({
        name: branchToEdit.name,
        address: branchToEdit.address,
        phone: branchToEdit.phone,
        email: branchToEdit.email,
      });
    } else {
      reset({
        name: "",
        address: "",
        phone: "",
        email: "",
      });
    }
  }, [branchToEdit, reset]);

  if (!isOpen) return null;

  const onSubmit = async (data: BranchFormData) => {
    try {
      if (branchToEdit) {
        await branchService.update(branchToEdit.id, data);
        toast.success("Branch updated successfully.");
      } else {
        await branchService.create(data);
        toast.success("Branch created successfully.");
      }
      reset();
      onSuccess();
      onClose();
    } catch (err: any) {
      const apiErrors = err.response?.data?.errors;
      const message = err.response?.data?.message || "Something went wrong.";

      if (apiErrors && typeof apiErrors === "object" && Object.keys(apiErrors).length > 0) {
        Object.keys(apiErrors).forEach((key) => {
          const fieldName = (key.charAt(0).toLowerCase() + key.slice(1)) as keyof BranchFormData;
          setError(fieldName, {
            type: "server",
            message: apiErrors[key][0],
          });
        });
      } else if (message.toLowerCase().includes("branch name") || message.toLowerCase().includes("named")) {
        setError("name", {
          type: "server",
          message: message,
        });
      } else if (message.toLowerCase().includes("email")) {
        setError("email", {
          type: "server",
          message: message,
        });
      } else if (message.toLowerCase().includes("phone")) {
        setError("phone", {
          type: "server",
          message: message,
        });
      } else if (message.toLowerCase().includes("address")) {
        setError("address", {
          type: "server",
          message: message,
        });
      } else {
        toast.error(message);
      }
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm">
      <div className="w-full max-w-lg bg-slate-900 border border-slate-700 rounded-2xl shadow-2xl overflow-hidden">
        <div className="flex items-center justify-between px-6 py-4 border-b border-slate-800 bg-slate-900/50">
          <h2 className="text-xl font-semibold text-white">
            {branchToEdit ? "Edit Branch" : "Add New Branch"}
          </h2>
          <button onClick={onClose} className="text-slate-400 hover:text-white transition-colors">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="p-6">
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-slate-300 mb-1">Branch Name</label>
              <input
                type="text"
                {...register("name")}
                className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                placeholder="e.g. Central Library"
              />
              {errors.name && <p className="text-red-400 text-xs mt-1">{errors.name.message}</p>}
            </div>

            <div>
              <label className="block text-sm font-medium text-slate-300 mb-1">Email</label>
              <input
                type="email"
                {...register("email")}
                className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                placeholder="branch@library.com"
              />
              {errors.email && <p className="text-red-400 text-xs mt-1">{errors.email.message}</p>}
            </div>

            <div>
              <label className="block text-sm font-medium text-slate-300 mb-1">Phone</label>
              <input
                type="text"
                {...register("phone")}
                className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                placeholder="+1 234 567 890"
              />
              {errors.phone && <p className="text-red-400 text-xs mt-1">{errors.phone.message}</p>}
            </div>

            <div>
              <label className="block text-sm font-medium text-slate-300 mb-1">Address</label>
              <textarea
                {...register("address")}
                rows={3}
                className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 resize-none"
                placeholder="Full address..."
              ></textarea>
              {errors.address && <p className="text-red-400 text-xs mt-1">{errors.address.message}</p>}
            </div>

            <div className="flex justify-end space-x-3 pt-4">
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
                {branchToEdit ? "Update Branch" : "Create Branch"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
