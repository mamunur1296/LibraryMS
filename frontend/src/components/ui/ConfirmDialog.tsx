interface ConfirmDialogProps {
  isOpen: boolean;
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  variant?: "danger" | "warning" | "default";
  onConfirm: () => void;
  onCancel: () => void;
}

export function ConfirmDialog({
  isOpen,
  title,
  message,
  confirmText = "Confirm",
  cancelText = "Cancel",
  variant = "default",
  onConfirm,
  onCancel,
}: ConfirmDialogProps) {
  if (!isOpen) return null;

  const confirmBtnStyle =
    variant === "danger"
      ? "bg-red-600 hover:bg-red-500 shadow-red-500/20"
      : variant === "warning"
      ? "bg-amber-600 hover:bg-amber-500 shadow-amber-500/20"
      : "bg-indigo-600 hover:bg-indigo-500 shadow-indigo-500/20";

  const iconBg =
    variant === "danger"
      ? "bg-red-500/10"
      : variant === "warning"
      ? "bg-amber-500/10"
      : "bg-indigo-500/10";

  const iconColor =
    variant === "danger" ? "text-red-400" : variant === "warning" ? "text-amber-400" : "text-indigo-400";

  const icon =
    variant === "danger" ? "⚠" : variant === "warning" ? "⚠" : "?";

  return (
    <div className="fixed inset-0 z-[100] flex items-center justify-center bg-black/70 backdrop-blur-sm">
      <div className="w-full max-w-sm bg-slate-900 border border-slate-700 rounded-2xl shadow-2xl overflow-hidden animate-[fadeInUp_0.2s_ease-out] mx-4">
        <div className="p-6">
          <div className={`w-12 h-12 rounded-full ${iconBg} flex items-center justify-center mb-4`}>
            <span className={`text-xl ${iconColor}`}>{icon}</span>
          </div>
          <h3 className="text-lg font-semibold text-white mb-2">{title}</h3>
          <p className="text-sm text-slate-400">{message}</p>
        </div>
        <div className="flex items-center justify-end gap-3 px-6 py-4 border-t border-slate-800 bg-slate-900/50">
          <button
            onClick={onCancel}
            className="px-4 py-2 rounded-lg text-sm font-medium text-slate-300 hover:text-white hover:bg-slate-800 transition-colors"
          >
            {cancelText}
          </button>
          <button
            onClick={onConfirm}
            className={`px-4 py-2 rounded-lg text-sm font-medium text-white shadow-lg transition-all ${confirmBtnStyle}`}
          >
            {confirmText}
          </button>
        </div>
      </div>
    </div>
  );
}
