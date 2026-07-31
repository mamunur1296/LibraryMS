import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { memberService } from "@/lib/services/member.service";
import { MemberDto } from "@/types/member.types";
import { toast } from "@/components/ui/Toast";

const renewMembershipSchema = z.object({
  days: z.number().min(1, "Minimum renew days is 1").max(365, "Maximum renew days is 365"),
});

type RenewMembershipFormData = z.infer<typeof renewMembershipSchema>;

interface RenewMembershipModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  member: MemberDto | null;
}

export function RenewMembershipModal({ isOpen, onClose, onSuccess, member }: RenewMembershipModalProps) {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<RenewMembershipFormData>({
    resolver: zodResolver(renewMembershipSchema),
    defaultValues: {
      days: 365,
    },
  });

  if (!isOpen || !member) return null;

  const onSubmit = async (data: RenewMembershipFormData) => {
    try {
      // Mock API call if backend endpoint doesn't exist yet, or use the real one
      // We saw {id}/renew in MembersController
      await memberService.renew(member.id, { days: data.days });
      toast.success('Membership renewed successfully.');
      reset();
      onSuccess();
      onClose();
    } catch (err: any) {
      toast.error(err.response?.data?.message || "Failed to renew membership");
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm">
      <div className="w-full max-w-md bg-slate-800 border border-slate-600 rounded-xl shadow-2xl overflow-hidden">
        <div className="flex items-center justify-between px-5 py-4 border-b border-slate-700 bg-slate-800">
          <h3 className="text-lg font-medium text-white">
            Renew Membership
          </h3>
          <button type="button" onClick={onClose} className="text-slate-400 hover:text-white transition-colors">
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="p-5">
          <div className="mb-4 bg-slate-900/50 p-4 rounded-xl border border-slate-700">
            <p className="text-sm text-slate-300 mb-1"><span className="font-medium text-slate-400">Member:</span> {member.fullName}</p>
            <p className="text-sm text-slate-300"><span className="font-medium text-slate-400">Current Status:</span> {member.status}</p>
          </div>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-slate-300 mb-1">Renew For (Days)</label>
              <input
                type="number"
                {...register("days", { valueAsNumber: true })}
                className="w-full px-3 py-2 bg-slate-900 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
              />
              {errors.days && <p className="text-red-400 text-xs mt-1">{errors.days.message}</p>}
            </div>

            <div className="flex justify-end space-x-3 pt-4 border-t border-slate-700">
              <button
                type="button"
                onClick={onClose}
                className="px-3 py-1.5 rounded-lg text-sm font-medium text-slate-300 hover:text-white hover:bg-slate-700 transition-colors"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={isSubmitting}
                className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg text-sm font-medium transition-colors shadow-lg shadow-indigo-500/20 disabled:opacity-50 flex items-center"
              >
                {isSubmitting ? (
                   <span className="mr-2 border-2 border-white/20 border-t-white rounded-full w-3 h-3 animate-spin"></span>
                ) : null}
                Renew Now
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
