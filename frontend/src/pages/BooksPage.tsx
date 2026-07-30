import { useEffect, useState, useCallback } from "react";
import { bookService } from "@/lib/services/book.service";
import { BookDto, PagedResult } from "@/types/book.types";
import { BookFormModal } from "@/components/books/BookFormModal";
import { AddCopiesModal } from "@/components/books/AddCopiesModal";
import { ViewCopiesModal } from "@/components/books/ViewCopiesModal";
import { ConfirmDialog } from "@/components/ui/ConfirmDialog";
import { toast } from "@/components/ui/Toast";

export default function BooksPage() {
  const [data, setData] = useState<PagedResult<BookDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [page, setPage] = useState(1);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [bookToEdit, setBookToEdit] = useState<BookDto | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<BookDto | null>(null);
  const [addCopiesTarget, setAddCopiesTarget] = useState<BookDto | null>(null);
  const [viewCopiesTarget, setViewCopiesTarget] = useState<BookDto | null>(null);

  const fetchBooks = useCallback(async () => {
    setLoading(true);
    try {
      const result = await bookService.search(searchTerm, undefined, undefined, undefined, page, 10);
      setData(result);
    } catch {
      toast.error("Failed to fetch books.");
    } finally {
      setLoading(false);
    }
  }, [searchTerm, page]);

  useEffect(() => {
    const delay = setTimeout(() => { void fetchBooks(); }, 300);
    return () => { clearTimeout(delay); };
  }, [fetchBooks]);

  const handleCreateNew = () => {
    setBookToEdit(null);
    setIsModalOpen(true);
  };

  const handleEdit = (book: BookDto) => {
    setBookToEdit(book);
    setIsModalOpen(true);
  };

  const handleAddCopies = (book: BookDto) => {
    setAddCopiesTarget(book);
  };

  const handleDeleteConfirm = async () => {
    if (!deleteTarget) return;
    try {
      await bookService.delete(deleteTarget.id);
      toast.success("Book deleted successfully.");
      void fetchBooks();
    } catch {
      toast.error("Failed to delete the book.");
    } finally {
      setDeleteTarget(null);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold tracking-tight text-white">Book Management</h1>
          <p className="text-sm text-slate-400 mt-1">Manage library catalog, categories, and authors.</p>
        </div>
        <button
          onClick={handleCreateNew}
          className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg shadow-lg shadow-indigo-500/20 text-sm font-medium transition-all flex items-center gap-2"
        >
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
          </svg>
          Add Book
        </button>
      </div>

      <div className="bg-slate-900 border border-slate-800 rounded-2xl overflow-hidden shadow-sm flex flex-col">
        <div className="p-4 border-b border-slate-800 flex justify-between items-center bg-slate-900/50">
          <div className="relative w-full max-w-md">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <svg className="h-5 w-5 text-slate-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </div>
            <input
              type="text"
              placeholder="Search books by title, author, or ISBN..."
              value={searchTerm}
              onChange={(e) => { setSearchTerm(e.target.value); setPage(1); }}
              className="block w-full pl-10 pr-3 py-2 border border-slate-700 rounded-lg bg-slate-950 text-slate-300 placeholder-slate-500 focus:outline-none focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
          </div>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm text-slate-300">
            <thead className="text-xs uppercase bg-slate-900 border-b border-slate-800 text-slate-400">
              <tr>
                <th className="px-6 py-4 font-medium">Book</th>
                <th className="px-6 py-4 font-medium">Category / Language</th>
                <th className="px-6 py-4 font-medium">Copies (Avail/Total)</th>
                <th className="px-6 py-4 font-medium">Published</th>
                <th className="px-6 py-4 text-right font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr><td colSpan={5} className="px-6 py-8 text-center">
                  <div className="flex items-center justify-center">
                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-500"></div>
                  </div>
                </td></tr>
              ) : !data || data.items.length === 0 ? (
                <tr><td colSpan={5} className="px-6 py-8 text-center text-slate-500">No books found matching your criteria.</td></tr>
              ) : (
                data.items.map((book) => (
                  <tr key={book.id} className="border-b border-slate-800/50 hover:bg-slate-800/20 transition-colors">
                    <td className="px-6 py-4">
                      <div className="font-medium text-white">{book.title}</div>
                      <div className="text-xs text-slate-500 mt-0.5">by {book.authorName} • ISBN: {book.isbn}</div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="text-slate-300">{book.categoryName}</div>
                      <div className="text-xs text-slate-500 mt-0.5">{book.language}</div>
                    </td>
                    <td className="px-6 py-4">
                      <span className={`font-medium ${book.availableCopies > 0 ? "text-emerald-400" : "text-red-400"}`}>{book.availableCopies}</span>
                      <span className="text-slate-500"> / {book.totalCopies}</span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-slate-400">{book.publicationYear}</td>
                    <td className="px-6 py-4 text-right space-x-3 whitespace-nowrap">
                      <button onClick={() => { setViewCopiesTarget(book); }} className="text-cyan-400 hover:text-cyan-300 text-xs font-medium" title="View Copies">
                        <svg className="w-4 h-4 inline mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" /><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" /></svg>
                        View
                      </button>
                      <button onClick={() => { handleAddCopies(book); }} className="text-emerald-400 hover:text-emerald-300 text-xs font-medium" title="Add Copies">+ Copies</button>
                      <button onClick={() => { handleEdit(book); }} className="text-indigo-400 hover:text-indigo-300 text-xs font-medium">Edit</button>
                      <button onClick={() => { setDeleteTarget(book); }} className="text-red-400 hover:text-red-300 text-xs font-medium">Delete</button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {data && (
          <div className="px-6 py-3 border-t border-slate-800 bg-slate-900/50 flex items-center justify-between">
            <div className="text-sm text-slate-400">
              Showing <span className="font-medium text-white">{data.totalCount === 0 ? 0 : (page - 1) * 10 + 1}</span> to{" "}
              <span className="font-medium text-white">{Math.min(page * 10, data.totalCount)}</span> of{" "}
              <span className="font-medium text-white">{data.totalCount}</span> results
            </div>
            <div className="flex space-x-1 items-center">
              <button onClick={() => { setPage((p) => Math.max(1, p - 1)); }} disabled={!data.hasPreviousPage} className="px-3 py-1 bg-slate-800 text-slate-300 rounded hover:bg-slate-700 disabled:opacity-50 text-sm border border-slate-700">Previous</button>
              
              <div className="hidden sm:flex space-x-1 mx-1">
                {Array.from({ length: Math.max(1, data.totalPages) }).map((_, i) => (
                  <button
                    key={i + 1}
                    onClick={() => setPage(i + 1)}
                    className={`px-3 py-1 rounded text-sm border ${
                      page === i + 1 
                        ? "bg-indigo-600 text-white border-indigo-500 shadow-sm shadow-indigo-500/20" 
                        : "bg-slate-800 text-slate-300 border-slate-700 hover:bg-slate-700"
                    }`}
                  >
                    {i + 1}
                  </button>
                ))}
              </div>

              <button onClick={() => { setPage((p) => Math.min(Math.max(1, data.totalPages), p + 1)); }} disabled={!data.hasNextPage} className="px-3 py-1 bg-slate-800 text-slate-300 rounded hover:bg-slate-700 disabled:opacity-50 text-sm border border-slate-700">Next</button>
            </div>
          </div>
        )}
      </div>

      <BookFormModal
        isOpen={isModalOpen}
        onClose={() => { setIsModalOpen(false); }}
        onSuccess={() => { void fetchBooks(); }}
        bookToEdit={bookToEdit}
      />

      <AddCopiesModal
        isOpen={addCopiesTarget !== null}
        onClose={() => { setAddCopiesTarget(null); }}
        onSuccess={() => { void fetchBooks(); }}
        book={addCopiesTarget}
      />
      
      <ViewCopiesModal
        isOpen={viewCopiesTarget !== null}
        onClose={() => { setViewCopiesTarget(null); }}
        book={viewCopiesTarget}
      />

      <ConfirmDialog
        isOpen={deleteTarget !== null}
        title="Delete Book"
        message={`Are you sure you want to delete "${deleteTarget?.title}"? This action cannot be undone.`}
        confirmText="Delete"
        variant="danger"
        onConfirm={() => { void handleDeleteConfirm(); }}
        onCancel={() => { setDeleteTarget(null); }}
      />
    </div>
  );
}
