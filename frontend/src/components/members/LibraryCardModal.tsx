import { MemberDto } from "@/types/member.types";

interface LibraryCardModalProps {
  isOpen: boolean;
  onClose: () => void;
  member: MemberDto | null;
}

export function LibraryCardModal({ isOpen, onClose, member }: LibraryCardModalProps) {
  if (!isOpen || !member) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm">
      <div className="w-full max-w-md bg-white rounded-2xl shadow-2xl overflow-hidden m-4">
        <div className="flex items-center justify-between px-6 py-4 border-b border-gray-200 bg-gray-50">
          <h2 className="text-xl font-semibold text-gray-800">Library Card</h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600 transition-colors print:hidden">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="p-6 bg-white">
          <div className="border-2 border-indigo-600 rounded-xl overflow-hidden relative" id="printable-card">
            <div className="bg-indigo-600 text-white p-4 flex justify-between items-center">
              <div className="font-bold text-lg">Central Library</div>
              <svg className="w-8 h-8 opacity-80" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
              </svg>
            </div>
            <div className="p-6 bg-white relative">
              <div className="flex justify-between items-start mb-6">
                <div>
                  <h3 className="text-xl font-bold text-gray-900">{member.fullName}</h3>
                  <p className="text-sm text-gray-500 mt-1">Member since {new Date(member.joinDate).getFullYear()}</p>
                </div>
                <div className="w-16 h-16 bg-gray-200 rounded-full flex items-center justify-center text-gray-400">
                  <svg className="w-8 h-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                  </svg>
                </div>
              </div>
              <div className="mb-2">
                <div className="text-xs text-gray-400 uppercase tracking-wider font-semibold">Membership ID</div>
                <div className="text-lg font-mono font-medium text-gray-800">{member.membershipNumber}</div>
              </div>
              
              {/* Fake Barcode */}
              <div className="mt-4 pt-4 border-t border-gray-100 flex justify-center">
                <div className="h-12 w-full max-w-[250px] bg-[repeating-linear-gradient(90deg,#000,#000_2px,transparent_2px,transparent_4px,#000_4px,#000_5px,transparent_5px,transparent_8px,#000_8px,#000_12px,transparent_12px,transparent_14px)] opacity-80"></div>
              </div>
            </div>
          </div>

          <div className="mt-6 flex justify-end print:hidden">
            <button
              onClick={() => window.print()}
              className="px-4 py-2 bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg text-sm font-medium transition-colors flex items-center gap-2"
            >
              <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 17h2a2 2 0 002-2v-4a2 2 0 00-2-2H5a2 2 0 00-2 2v4a2 2 0 002 2h2m2 4h6a2 2 0 002-2v-4a2 2 0 00-2-2H9a2 2 0 00-2 2v4a2 2 0 002 2zm8-12V5a2 2 0 00-2-2H9a2 2 0 00-2 2v4h10z" />
              </svg>
              Print Card
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
