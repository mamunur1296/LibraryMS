import { Link, useNavigate } from "react-router-dom";
import { Heart, Check, X, ShieldAlert } from "lucide-react";
import { BookDto } from "../../types/book.types";
import { Button } from "./Button";
import { Badge } from "./Badge";
import { cn } from "../../lib/utils";
import { useAuth } from "../../contexts/AuthContext";
import { favouriteService } from "../../lib/services/favourite.service";
import { useState, useEffect } from "react";
import { useToast } from "./Toast";

interface BookCardProps {
  book: BookDto;
  viewMode?: "grid" | "list";
  onLoginPrompt?: () => void;
}

export function BookCard({ book, viewMode = "grid", onLoginPrompt }: BookCardProps) {
  const { user } = useAuth();
  const { addToast } = useToast();
  const navigate = useNavigate();
  const [isFavourite, setIsFavourite] = useState(false);
  const [isHovered, setIsHovered] = useState(false);

  useEffect(() => {
    if (!user) {
      setIsFavourite(favouriteService.isLocalFavourite(book.id));
    } else {
      // Need to rely on a passed prop or fetch individually.
      // For now we assume local storage syncs or is updated by parent for logged-in.
      // A better way is to handle favorites context, but we will just use local storage for guest
      setIsFavourite(favouriteService.isLocalFavourite(book.id));
    }
  }, [book.id, user]);

  const toggleFavourite = async (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();

    if (!user) {
      const isFav = favouriteService.toggleLocalFavourite(book.id);
      setIsFavourite(isFav);
      addToast("success", isFav ? "Added to favourites" : "Removed from favourites");
    } else {
      try {
        if (isFavourite) {
          await favouriteService.removeFavourite(book.id);
          setIsFavourite(false);
          favouriteService.toggleLocalFavourite(book.id); // sync local
        } else {
          await favouriteService.addFavourite(book.id);
          setIsFavourite(true);
          favouriteService.toggleLocalFavourite(book.id); // sync local
        }
      } catch (error) {
        addToast("error", "Failed to update favourite");
      }
    }
  };

  const handleAction = (e: React.MouseEvent, action: "borrow" | "reserve") => {
    e.preventDefault();
    if (!user) {
      if (onLoginPrompt) {
        onLoginPrompt();
      } else {
        navigate("/login");
      }
      return;
    }

    if (action === "borrow") {
<<<<<<< Updated upstream
      navigate(`/dashboard/borrows/new?bookId=${book.id}`);
    } else {
      navigate(`/dashboard/reservations/new?bookId=${book.id}`);
=======
      navigate(`/borrows?action=new&bookId=${book.id}`);
    } else {
      navigate(`/reservations?action=new&bookId=${book.id}`);
>>>>>>> Stashed changes
    }
  };

  const isGrid = viewMode === "grid";

  return (
    <Link
      to={`/catalog/${book.id}`}
      className={cn(
        "group relative flex overflow-hidden rounded-xl border bg-card text-card-foreground shadow-sm transition-all hover:shadow-md",
        isGrid ? "flex-col" : "flex-row h-48"
      )}
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
    >
      {/* Cover Image */}
      <div
        className={cn(
          "relative overflow-hidden bg-muted/30",
          isGrid ? "aspect-[3/4] w-full" : "h-full w-32 shrink-0 md:w-40"
        )}
      >
        {book.coverImageUrl ? (
          <img
            src={book.coverImageUrl}
            alt={book.title}
            className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-105"
          />
        ) : (
          <div className="flex h-full w-full items-center justify-center bg-muted">
            <span className="text-muted-foreground text-4xl">📚</span>
          </div>
        )}
        
        {/* Favourite Button */}
        <button
          onClick={toggleFavourite}
          className={cn(
            "absolute right-2 top-2 rounded-full p-2 backdrop-blur-md transition-all",
            isFavourite 
              ? "bg-red-500/90 text-white" 
              : "bg-background/50 text-foreground hover:bg-background/80",
            (isHovered || isFavourite) ? "opacity-100" : "opacity-0 md:opacity-0 opacity-100" // Show on mobile or hover
          )}
        >
          <Heart className={cn("h-4 w-4", isFavourite && "fill-current")} />
        </button>
      </div>

      {/* Content */}
      <div className={cn("flex flex-1 flex-col justify-between p-4", isGrid ? "gap-2" : "py-4")}>
        <div>
          <div className="flex items-start justify-between gap-2">
            <div>
              <h3 className="font-semibold leading-tight line-clamp-1 group-hover:text-primary transition-colors">
                {book.title}
              </h3>
              <p className="text-sm text-muted-foreground mt-1 line-clamp-1">
                by {book.authorName}
              </p>
            </div>
          </div>
          
          <div className="mt-2 flex flex-wrap gap-2">
            <Badge variant="secondary" className="text-xs font-normal">
              {book.categoryName}
            </Badge>
            <span className="text-xs text-muted-foreground flex items-center">
              ISBN: {book.isbn}
            </span>
          </div>
        </div>

        <div className={cn("mt-4 flex items-end justify-between", isGrid ? "mt-4" : "mt-0")}>
          <div className="space-y-1">
            <div className="flex items-center gap-1.5 text-sm">
              {book.availableCopies > 0 ? (
                <>
                  <div className="flex h-4 w-4 items-center justify-center rounded-full bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400">
                    <Check className="h-3 w-3" />
                  </div>
                  <span className="font-medium">{book.availableCopies} available</span>
                </>
              ) : (
                <>
                  <div className="flex h-4 w-4 items-center justify-center rounded-full bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400">
                    <X className="h-3 w-3" />
                  </div>
                  <span className="text-muted-foreground">Not available</span>
                </>
              )}
            </div>
            <p className="text-xs text-muted-foreground">
              Total copies: {book.totalCopies}
            </p>
          </div>

          <div className={cn("flex gap-2 transition-opacity", !isGrid && "opacity-100", isHovered ? "opacity-100" : "opacity-100 md:opacity-0")}>
            {book.availableCopies > 0 ? (
<<<<<<< Updated upstream
              <Button size="sm" onClick={(e) => handleAction(e, "borrow")}>
                Borrow
              </Button>
            ) : (
              <Button size="sm" variant="outline" onClick={(e) => handleAction(e, "reserve")}>
=======
              <Button size="sm" onClick={(e) => {
                e.preventDefault();
                e.stopPropagation();
                handleAction(e, "borrow");
              }}>
                Borrow
              </Button>
            ) : (
              <Button size="sm" variant="outline" onClick={(e) => {
                e.preventDefault();
                e.stopPropagation();
                handleAction(e, "reserve");
              }}>
>>>>>>> Stashed changes
                Reserve
              </Button>
            )}
          </div>
        </div>
      </div>
    </Link>
  );
}
