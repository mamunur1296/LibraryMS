import { MemberDto } from "@/types/member.types";

interface MemberProfileModalProps {
  isOpen: boolean;
  onClose: () => void;
  member: MemberDto | null;
}

export function MemberProfileModal({ isOpen, onClose, member }: MemberProfileModalProps) {
  if (!isOpen || !member) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-end bg-black/60 backdrop-blur-sm">
      <div className="w-full max-w-md bg-slate-900 border-l border-slate-700 h-full overflow-y-auto shadow-2xl animate-slide-in-right">
        <div className="flex items-center justify-between px-6 py-4 border-b border-slate-800 bg-slate-900/50 sticky top-0 z-10">
          <h2 className="text-xl font-semibold text-white">Member Profile</h2>
          <button onClick={onClose} className="text-slate-400 hover:text-white transition-colors">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="p-6 space-y-8">
          {/* Header Info */}
          <div className="flex items-center gap-4">
            <div className="w-16 h-16 rounded-full bg-indigo-500/20 text-indigo-400 flex items-center justify-center text-2xl font-bold uppercase">
              {member.fullName.charAt(0)}
            </div>
            <div>
              <h3 className="text-xl font-bold text-white">{member.fullName}</h3>
              <p className="text-sm text-slate-400">ID: {member.membershipNumber}</p>
              <span className={`inline-flex items-center px-2 py-0.5 rounded text-xs font-medium mt-2 border ${member.status === "Active" ? "bg-emerald-500/10 text-emerald-400 border-emerald-500/20" : member.status === "Suspended" ? "bg-amber-500/10 text-amber-400 border-amber-500/20" : "bg-red-500/10 text-red-400 border-red-500/20"}`}>
                {member.status}
              </span>
            </div>
          </div>

          {/* Contact Details */}
          <div>
            <h4 className="text-sm font-semibold text-slate-300 uppercase tracking-wider mb-3">Contact Information</h4>
            <div className="space-y-3 bg-slate-950 p-4 rounded-xl border border-slate-800">
              <div className="flex justify-between items-center">
                <span className="text-sm text-slate-500">Email</span>
                <span className="text-sm font-medium text-slate-300">{member.email}</span>
              </div>
              <div className="flex justify-between items-center">
                <span className="text-sm text-slate-500">Phone</span>
                <span className="text-sm font-medium text-slate-300">{member.phone}</span>
              </div>
              <div className="flex justify-between items-start">
                <span className="text-sm text-slate-500">Address</span>
                <span className="text-sm font-medium text-slate-300 text-right max-w-[200px]">{member.address}</span>
              </div>
            </div>
          </div>

          {/* Library Activity */}
          <div>
            <h4 className="text-sm font-semibold text-slate-300 uppercase tracking-wider mb-3">Library Activity</h4>
            <div className="grid grid-cols-2 gap-3">
              <div className="bg-slate-950 p-4 rounded-xl border border-slate-800 text-center">
                <span className="block text-2xl font-bold text-indigo-400">{member.activeBorrows}</span>
                <span className="text-xs text-slate-500 mt-1 block">Active Borrows</span>
              </div>
              <div className="bg-slate-950 p-4 rounded-xl border border-slate-800 text-center">
                <span className="block text-2xl font-bold text-white">{new Date(member.joinDate).toLocaleDateString()}</span>
                <span className="text-xs text-slate-500 mt-1 block">Joined Date</span>
              </div>
            </div>
          </div>
          
          {/* Account Status */}
          <div>
            <h4 className="text-sm font-semibold text-slate-300 uppercase tracking-wider mb-3">System Account</h4>
            <div className="bg-slate-950 p-4 rounded-xl border border-slate-800">
              <div className="flex items-center gap-2 mb-2">
                <div className={`w-2 h-2 rounded-full ${member.hasAccount ? 'bg-emerald-400' : 'bg-rose-400'}`}></div>
                <span className="text-sm font-medium text-slate-300">
                  {member.hasAccount ? 'Online Account Linked' : 'No Online Account'}
                </span>
              </div>
              {member.hasAccount && (
                <p className="text-xs text-slate-500">User can log in to view their own fines, reservations, and borrow history.</p>
              )}
            </div>
          </div>

        </div>
      </div>
    </div>
  );
}
