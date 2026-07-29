"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { bookSchema, BookFormData } from "@/lib/validations/book.schema";
import { bookService } from "@/lib/services/book.service";
import { branchService } from "@/lib/services/branch.service";
import { BookDto, AuthorDto, CategoryDto } from "@/types/book.types";
import { BranchDto } from "@/types/branch.types";
import { useEffect, useState } from "react";
import { AddEntityModal } from "./AddEntityModal";

interface BookFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  bookToEdit?: BookDto | null;
}

export function BookFormModal({ isOpen, onClose, onSuccess, bookToEdit }: BookFormModalProps) {
  const [authors, setAuthors] = useState<AuthorDto[]>([]);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [branches, setBranches] = useState<BranchDto[]>([]);
  const [loadingData, setLoadingData] = useState(false);
  
  // State for sub-modals
  const [isAddAuthorOpen, setIsAddAuthorOpen] = useState(false);
  const [isAddCategoryOpen, setIsAddCategoryOpen] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<BookFormData>({
    resolver: zodResolver(bookSchema),
  });

  const fetchData = async () => {
    setLoadingData(true);
    try {
      const [authorsData, categoriesData, branchesData] = await Promise.all([
        bookService.getAllAuthors(),
        bookService.getAllCategories(),
        branchService.getAll(false), // only active branches
      ]);
      setAuthors(authorsData);
      setCategories(categoriesData);
      setBranches(branchesData);
    } catch (error) {
      console.error("Failed to load form data", error);
    } finally {
      setLoadingData(false);
    }
  };

  useEffect(() => {
    if (isOpen) {
      fetchData();
      if (bookToEdit) {
        reset({
          title: bookToEdit.title,
          isbn: bookToEdit.isbn,
          description: bookToEdit.description || "",
          publicationYear: bookToEdit.publicationYear,
          categoryId: bookToEdit.categoryId,
          authorId: bookToEdit.authorId,
          language: bookToEdit.language,
          // these are ignored on update in backend but required by schema, we could conditionally refine schema but for now we'll pass dummy or defaults
          initialCopies: 1,
          branchId: branches[0]?.id || "",
        });
      } else {
        reset({
          title: "",
          isbn: "",
          description: "",
          publicationYear: new Date().getFullYear(),
          categoryId: "",
          authorId: "",
          language: "English",
          initialCopies: 1,
          branchId: "",
        });
      }
    }
  }, [isOpen, bookToEdit, reset]);

  if (!isOpen) return null;

  const onSubmit = async (data: BookFormData) => {
    try {
      if (bookToEdit) {
        await bookService.update(bookToEdit.id, data);
      } else {
        await bookService.create({
          ...data,
          branchId: data.branchId!,
          initialCopies: data.initialCopies!,
        });
      }
      reset();
      onSuccess();
      onClose();
    } catch (err: any) {
      const apiErrors = err.response?.data?.errors;
      if (apiErrors && typeof apiErrors === "object") {
        Object.keys(apiErrors).forEach((key) => {
          const fieldName = (key.charAt(0).toLowerCase() + key.slice(1)) as keyof BookFormData;
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
    <>
      <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm overflow-y-auto pt-10 pb-10">
        <div className="w-full max-w-2xl bg-slate-900 border border-slate-700 rounded-2xl shadow-2xl overflow-hidden m-4">
          <div className="flex items-center justify-between px-6 py-4 border-b border-slate-800 bg-slate-900/50 sticky top-0 z-10">
            <h2 className="text-xl font-semibold text-white">
              {bookToEdit ? "Edit Book" : "Add New Book"}
            </h2>
            <button onClick={onClose} className="text-slate-400 hover:text-white transition-colors">
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <div className="p-6">
            {loadingData ? (
              <div className="flex justify-center items-center h-40">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-500"></div>
              </div>
            ) : (
              <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  {/* Basic Info */}
                  <div className="md:col-span-2">
                    <label className="block text-sm font-medium text-slate-300 mb-1">Title</label>
                    <input
                      type="text"
                      {...register("title")}
                      className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                    />
                    {errors.title && <p className="text-red-400 text-xs mt-1">{errors.title.message}</p>}
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-slate-300 mb-1">ISBN</label>
                    <input
                      type="text"
                      {...register("isbn")}
                      className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                    />
                    {errors.isbn && <p className="text-red-400 text-xs mt-1">{errors.isbn.message}</p>}
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-slate-300 mb-1">Publication Year</label>
                    <input
                      type="number"
                      {...register("publicationYear", { valueAsNumber: true })}
                      className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                    />
                    {errors.publicationYear && <p className="text-red-400 text-xs mt-1">{errors.publicationYear.message}</p>}
                  </div>

                  {/* Author and Category Dropdowns */}
                  <div>
                    <div className="flex justify-between items-end mb-1">
                      <label className="block text-sm font-medium text-slate-300">Author</label>
                      <button type="button" onClick={() => setIsAddAuthorOpen(true)} className="text-xs text-indigo-400 hover:text-indigo-300 font-medium">+ Add New</button>
                    </div>
                    <select
                      {...register("authorId")}
                      className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 appearance-none"
                    >
                      <option value="">Select Author...</option>
                      {authors.map(a => <option key={a.id} value={a.id}>{a.name}</option>)}
                    </select>
                    {errors.authorId && <p className="text-red-400 text-xs mt-1">{errors.authorId.message}</p>}
                  </div>

                  <div>
                    <div className="flex justify-between items-end mb-1">
                      <label className="block text-sm font-medium text-slate-300">Category</label>
                      <button type="button" onClick={() => setIsAddCategoryOpen(true)} className="text-xs text-indigo-400 hover:text-indigo-300 font-medium">+ Add New</button>
                    </div>
                    <select
                      {...register("categoryId")}
                      className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 appearance-none"
                    >
                      <option value="">Select Category...</option>
                      {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                    </select>
                    {errors.categoryId && <p className="text-red-400 text-xs mt-1">{errors.categoryId.message}</p>}
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-slate-300 mb-1">Language</label>
                    <input
                      type="text"
                      {...register("language")}
                      className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                    />
                    {errors.language && <p className="text-red-400 text-xs mt-1">{errors.language.message}</p>}
                  </div>

                  {/* Creation Specific Fields */}
                  {!bookToEdit && (
                    <>
                      <div>
                        <label className="block text-sm font-medium text-slate-300 mb-1">Initial Copies</label>
                        <input
                          type="number"
                          {...register("initialCopies", { valueAsNumber: true })}
                          className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                        />
                        {errors.initialCopies && <p className="text-red-400 text-xs mt-1">{errors.initialCopies.message}</p>}
                      </div>
                      <div className="md:col-span-2">
                        <label className="block text-sm font-medium text-slate-300 mb-1">Assign to Branch</label>
                        <select
                          {...register("branchId")}
                          className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 appearance-none"
                        >
                          <option value="">Select Branch...</option>
                          {branches.map(b => <option key={b.id} value={b.id}>{b.name}</option>)}
                        </select>
                        {errors.branchId && <p className="text-red-400 text-xs mt-1">{errors.branchId.message}</p>}
                      </div>
                    </>
                  )}

                  <div className="md:col-span-2">
                    <label className="block text-sm font-medium text-slate-300 mb-1">Description</label>
                    <textarea
                      {...register("description")}
                      rows={3}
                      className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 resize-none"
                    ></textarea>
                    {errors.description && <p className="text-red-400 text-xs mt-1">{errors.description.message}</p>}
                  </div>
                </div>

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
                    {bookToEdit ? "Update Book" : "Create Book"}
                  </button>
                </div>
              </form>
            )}
          </div>
        </div>
      </div>

      <AddEntityModal
        isOpen={isAddAuthorOpen}
        onClose={() => setIsAddAuthorOpen(false)}
        onSuccess={fetchData}
        entityType="author"
      />
      
      <AddEntityModal
        isOpen={isAddCategoryOpen}
        onClose={() => setIsAddCategoryOpen(false)}
        onSuccess={fetchData}
        entityType="category"
      />
    </>
  );
}
