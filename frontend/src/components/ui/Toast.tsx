import { useState, useCallback, useEffect } from "react";

export type ToastType = "success" | "error" | "warning" | "info";

export interface Toast {
  id: string;
  type: ToastType;
  message: string;
}

interface ToastItemProps {
  toast: Toast;
  onRemove: (id: string) => void;
}

const icons: Record<ToastType, string> = {
  success: "✓",
  error: "✕",
  warning: "⚠",
  info: "ℹ",
};

const styles: Record<ToastType, string> = {
  success: "bg-emerald-900/80 border-emerald-500/40 text-emerald-200",
  error: "bg-red-900/80 border-red-500/40 text-red-200",
  warning: "bg-amber-900/80 border-amber-500/40 text-amber-200",
  info: "bg-blue-900/80 border-blue-500/40 text-blue-200",
};

const iconStyles: Record<ToastType, string> = {
  success: "bg-emerald-500 text-white",
  error: "bg-red-500 text-white",
  warning: "bg-amber-500 text-white",
  info: "bg-blue-500 text-white",
};

function ToastItem({ toast, onRemove }: ToastItemProps) {
  useEffect(() => {
    const timer = setTimeout(() => { onRemove(toast.id); }, 4000);
    return () => { clearTimeout(timer); };
  }, [toast.id, onRemove]);

  return (
    <div
      className={`flex items-center gap-3 px-4 py-3 rounded-xl border backdrop-blur-sm shadow-2xl min-w-[280px] max-w-sm animate-[fadeInUp_0.3s_ease-out] ${styles[toast.type]}`}
    >
      <span className={`flex-shrink-0 w-6 h-6 rounded-full flex items-center justify-center text-xs font-bold ${iconStyles[toast.type]}`}>
        {icons[toast.type]}
      </span>
      <span className="text-sm font-medium flex-1">{toast.message}</span>
      <button
        onClick={() => { onRemove(toast.id); }}
        className="flex-shrink-0 opacity-60 hover:opacity-100 transition-opacity"
      >
        ✕
      </button>
    </div>
  );
}

interface ToastContainerProps {
  toasts: Toast[];
  onRemove: (id: string) => void;
}

export function ToastContainer({ toasts, onRemove }: ToastContainerProps) {
  if (toasts.length === 0) return null;
  return (
    <div className="fixed top-4 right-4 z-[9999] flex flex-col gap-2">
      {toasts.map((t) => (
        <ToastItem key={t.id} toast={t} onRemove={onRemove} />
      ))}
    </div>
  );
}

// Hook
let _addToast: ((type: ToastType, message: string) => void) | null = null;

export function useToast() {
  const [toasts, setToasts] = useState<Toast[]>([]);

  const addToast = useCallback((type: ToastType, message: string) => {
    const id = Math.random().toString(36).slice(2);
    setToasts((prev) => [...prev, { id, type, message }]);
  }, []);

  const removeToast = useCallback((id: string) => {
    setToasts((prev) => prev.filter((t) => t.id !== id));
  }, []);

  // expose globally so non-component code can call it
  useEffect(() => {
    _addToast = addToast;
    return () => { _addToast = null; };
  }, [addToast]);

  return { toasts, addToast, removeToast };
}

// Global convenience functions (call from anywhere in page components)
export const toast = {
  success: (message: string) => { _addToast?.("success", message); },
  error: (message: string) => { _addToast?.("error", message); },
  warning: (message: string) => { _addToast?.("warning", message); },
  info: (message: string) => { _addToast?.("info", message); },
};
