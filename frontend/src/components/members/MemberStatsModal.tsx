import { useEffect, useState } from "react";
import { memberService } from "@/lib/services/member.service";
import { MemberDto, MemberProfileStatsDto } from "@/types/member.types";
import { toast } from "@/components/ui/Toast";

interface MemberStatsModalProps {
  isOpen: boolean;
  onClose: () => void;
  member: MemberDto | null;
}

export function MemberStatsModal({ isOpen, onClose, member }: MemberStatsModalProps) {
  const [stats, setStats] = useState<MemberProfileStatsDto | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (isOpen && member) {
      setLoading(true);
      memberService.getStats(member.id)
        .then((data) => {
          setStats(data);
        })
        .catch((err) => {
          toast.error("Failed to load member stats.");
          console.error(err);
        })
        .finally(() => {
          setLoading(false);
        });
    } else {
      setStats(null);
    }
  }, [isOpen, member]);

  if (!isOpen || !member) return null;

  return (
    <div className="fixed inset-0 z-[60] flex items-center justify-center bg-black/60 backdrop-blur-sm">
      <div className="w-full max-w-md bg-slate-800 border border-slate-700 rounded-xl shadow-2xl overflow-hidden">
        <div className="flex items-center justify-between px-5 py-3 border-b border-slate-700 bg-slate-800">
          <h3 className="text-lg font-medium text-white">
            Member Stats: {member.fullName}
          </h3>
          <button type="button" onClick={onClose} className="text-slate-400 hover:text-white transition-colors">
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="p-5">
          {loading ? (
            <div className="flex items-center justify-center py-8">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-500"></div>
            </div>
          ) : stats ? (
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div className="p-4 bg-slate-900 border border-slate-750 rounded-xl text-center">
                  <div className="text-2xl font-bold text-white">{stats.totalBorrows}</div>
                  <div className="text-xs text-slate-400 mt-1">Total Borrows</div>
                </div>

                <div className="p-4 bg-slate-900 border border-slate-750 rounded-xl text-center">
                  <div className="text-2xl font-bold text-indigo-400">{stats.activeBorrows}</div>
                  <div className="text-xs text-slate-400 mt-1">Active Borrows</div>
                </div>

                <div className="p-4 bg-slate-900 border border-slate-750 rounded-xl text-center">
                  <div className="text-2xl font-bold text-rose-450">{stats.overdueBorrows}</div>
                  <div className="text-xs text-slate-400 mt-1">Overdue Borrows</div>
                </div>

                <div className="p-4 bg-slate-900 border border-slate-750 rounded-xl text-center">
                  <div className="text-2xl font-bold text-emerald-450">{stats.activeReservations}</div>
                  <div className="text-xs text-slate-400 mt-1">Active Reservations</div>
                </div>
              </div>

              <div className="p-4 bg-slate-900/50 border border-slate-750 rounded-xl flex items-center justify-between">
                <div>
                  <div className="text-sm font-medium text-white">Outstanding Fines</div>
                  <div className="text-xs text-slate-400 mt-0.5">Total unpaid penalty fines</div>
                </div>
                <div className="text-xl font-extrabold text-amber-400">
                  ${stats.totalFinesDue.toFixed(2)}
                </div>
              </div>

              <div className="flex justify-end pt-2">
                <button
                  type="button"
                  onClick={onClose}
                  className="px-4 py-2 bg-slate-700 hover:bg-slate-650 text-white rounded-lg text-sm font-medium transition-colors"
                >
                  Close
                </button>
              </div>
            </div>
          ) : (
            <div className="text-center py-6 text-slate-400">
              No stats available.
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
