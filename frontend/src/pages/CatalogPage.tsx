import { useState, useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import { LayoutGrid, List } from "lucide-react";
import { BookDto } from "../types/book.types";
import { publicService } from "../lib/services/public.service";
import { SearchInput } from "../components/ui/SearchInput";
import { BranchSelector } from "../components/ui/BranchSelector";
import { BookCard } from "../components/ui/BookCard";
import { Pagination } from "../components/ui/Pagination";
import { Spinner } from "../components/ui/Spinner";
import { EmptyState } from "../components/ui/EmptyState";
import { Button } from "../components/ui/Button";

export default function CatalogPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [books, setBooks] = useState<BookDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  const [viewMode, setViewMode] = useState<"grid" | "list">("grid");

  const page = parseInt(searchParams.get("page") || "1");
  const searchTerm = searchParams.get("search") || "";
  const branchId = searchParams.get("branch") || "";
  const pageSize = 12;

  useEffect(() => {
    const fetchBooks = async () => {
      setIsLoading(true);
      try {
        const response = await publicService.getBooks({
          page,
          pageSize,
          searchTerm: searchTerm || undefined,
          branchId: branchId && branchId !== "all" ? branchId : undefined,
        });
        setBooks(response.items);
        setTotalCount(response.totalCount);
      } catch (error) {
        console.error("Failed to fetch books", error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchBooks();
  }, [page, searchTerm, branchId]);

  const updateSearchParam = (key: string, value: string) => {
    const newParams = new URLSearchParams(searchParams);
    if (value) {
      newParams.set(key, value);
    } else {
      newParams.delete(key);
    }
    if (key !== "page") {
      newParams.set("page", "1"); // Reset page on new filter
    }
    setSearchParams(newParams);
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="container mx-auto px-4 py-8 max-w-7xl">
      <div className="mb-8 flex flex-col md:flex-row md:items-end justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Library Catalog</h1>
          <p className="text-muted-foreground mt-1">
            Browse and discover our collection of books
          </p>
        </div>

        <div className="flex flex-col sm:flex-row gap-3">
          <BranchSelector
            value={branchId}
            onChange={(val) => updateSearchParam("branch", val)}
            allowAll
            className="w-full sm:w-[200px]"
          />
          <SearchInput
            placeholder="Search books, authors..."
            value={searchTerm}
            onSearch={(val) => updateSearchParam("search", val)}
            className="w-full sm:w-[300px]"
          />
          <div className="hidden sm:flex border rounded-md">
            <Button
              variant={viewMode === "grid" ? "secondary" : "ghost"}
              size="icon"
              className="rounded-none rounded-l-md"
              onClick={() => setViewMode("grid")}
            >
              <LayoutGrid className="h-4 w-4" />
            </Button>
            <Button
              variant={viewMode === "list" ? "secondary" : "ghost"}
              size="icon"
              className="rounded-none rounded-r-md"
              onClick={() => setViewMode("list")}
            >
              <List className="h-4 w-4" />
            </Button>
          </div>
        </div>
      </div>

      {isLoading ? (
        <div className="flex min-h-[400px] items-center justify-center">
          <Spinner size="lg" />
        </div>
      ) : books.length > 0 ? (
        <>
          <div
            className={
              viewMode === "grid"
                ? "grid grid-cols-1 gap-6 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4"
                : "flex flex-col gap-4"
            }
          >
            {books.map((book) => (
              <BookCard key={book.id} book={book} viewMode={viewMode} />
            ))}
          </div>

          <div className="mt-8">
            <Pagination
              currentPage={page}
              totalPages={totalPages}
              onPageChange={(p) => updateSearchParam("page", p.toString())}
            />
          </div>
        </>
      ) : (
        <EmptyState
          title="No books found"
          description="Try adjusting your search or filters to find what you're looking for."
          action={
            <Button variant="outline" onClick={() => setSearchParams({})}>
              Clear all filters
            </Button>
          }
        />
      )}
    </div>
  );
}
