import { ChevronLeft, ChevronRight, MoreHorizontal } from "lucide-react";
import { cn } from "../../lib/utils";
import { Button } from "./Button";

interface PaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  className?: string;
}

export function Pagination({
  currentPage,
  totalPages,
  onPageChange,
  className,
}: PaginationProps) {
  if (totalPages <= 1) return null;

  const generatePages = () => {
    const pages: (number | string)[] = [];
    const maxVisiblePages = 5;

    if (totalPages <= maxVisiblePages) {
      for (let i = 1; i <= totalPages; i++) {
        pages.push(i);
      }
    } else {
      pages.push(1);
      
      if (currentPage > 3) {
        pages.push("ellipsis-start");
      }
      
      const start = Math.max(2, currentPage - 1);
      const end = Math.min(totalPages - 1, currentPage + 1);
      
      for (let i = start; i <= end; i++) {
        pages.push(i);
      }
      
      if (currentPage < totalPages - 2) {
        pages.push("ellipsis-end");
      }
      
      pages.push(totalPages);
    }
    
    return pages;
  };

  return (
    <nav
      role="navigation"
      aria-label="pagination"
      className={cn("mx-auto flex w-full justify-center", className)}
    >
      <ul className="flex flex-row items-center gap-1">
        <li>
          <Button
            variant="outline"
            size="sm"
            onClick={() => onPageChange(currentPage - 1)}
            disabled={currentPage <= 1}
            className="gap-1 pl-2.5"
          >
            <ChevronLeft className="h-4 w-4" />
            <span>Previous</span>
          </Button>
        </li>
        
        {generatePages().map((page, index) => {
          if (typeof page === "string") {
            return (
              <li key={`${page}-${index}`} className="flex h-9 w-9 items-center justify-center">
                <MoreHorizontal className="h-4 w-4 text-muted-foreground" />
              </li>
            );
          }
          
          return (
            <li key={page}>
              <Button
                variant={currentPage === page ? "primary" : "outline"}
                size="sm"
                className={cn("w-9", currentPage === page && "pointer-events-none")}
                onClick={() => onPageChange(page)}
              >
                {page}
              </Button>
            </li>
          );
        })}
        
        <li>
          <Button
            variant="outline"
            size="sm"
            onClick={() => onPageChange(currentPage + 1)}
            disabled={currentPage >= totalPages}
            className="gap-1 pr-2.5"
          >
            <span>Next</span>
            <ChevronRight className="h-4 w-4" />
          </Button>
        </li>
      </ul>
    </nav>
  );
}
