import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { authorSchema, categorySchema, AuthorFormData, CategoryFormData } from "@/lib/validations/book.schema";
import { bookService } from "@/lib/services/book.service";

interface AddEntityModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  entityType: "author" | "category";
}

export function AddEntityModal({ isOpen, onClose, onSuccess, entityType }: AddEntityModalProps) {
  const isAuthor = entityType === "author";
  const schema = isAuthor ? authorSchema : categorySchema;
  
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<any>({
    resolver: zodResolver(schema),
    defaultValues: { name: "", biography: "", description: "" },
  });

  if (!isOpen) return null;

  const onSubmit = async (data: any) => {
    try {
      if (isAuthor) {
        await bookService.createAuthor(data as AuthorFormData);
      } else {
        await bookService.createCategory(data as CategoryFormData);
      }
      reset();
      onSuccess();
      onClose();
    } catch (err: any) {
      alert(err.response?.data?.message || `Failed to create ${entityType}`);
    }
  };

  return (
    <div className="fixed inset-0 z-[60] flex items-center justify-center bg-black/60 backdrop-blur-sm">
      <div className="w-full max-w-sm bg-slate-800 border border-slate-600 rounded-xl shadow-2xl overflow-hidden">
        <div className="flex items-center justify-between px-5 py-3 border-b border-slate-700 bg-slate-800">
          <h3 className="text-lg font-medium text-white">
            Add New {isAuthor ? "Author" : "Category"}
          </h3>
          <button type="button" onClick={onClose} className="text-slate-400 hover:text-white transition-colors">
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="p-5">
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-slate-300 mb-1">Name</label>
              <input
                type="text"
                {...register("name")}
                className="w-full px-3 py-2 bg-slate-900 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                placeholder={`e.g. ${isAuthor ? "J.K. Rowling" : "Science Fiction"}`}
              />
              {errors.name && <p className="text-red-400 text-xs mt-1">{errors.name.message as string}</p>}
            </div>

            <div>
              <label className="block text-sm font-medium text-slate-300 mb-1">
                {isAuthor ? "Biography (Optional)" : "Description (Optional)"}
              </label>
              <textarea
                {...register(isAuthor ? "biography" : "description")}
                rows={3}
                className="w-full px-3 py-2 bg-slate-900 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 resize-none"
                placeholder="..."
              ></textarea>
            </div>

            <div className="flex justify-end space-x-3 pt-2">
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
                className="px-3 py-1.5 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg text-sm font-medium transition-colors shadow-lg disabled:opacity-50 flex items-center"
              >
                {isSubmitting ? (
                   <span className="mr-2 border-2 border-white/20 border-t-white rounded-full w-3 h-3 animate-spin"></span>
                ) : null}
                Save
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
