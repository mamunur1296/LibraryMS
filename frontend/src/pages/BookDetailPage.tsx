import { useEffect, useState } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import { ArrowLeft, BookOpen, Check, Heart, User, Building } from "lucide-react";
import { BookDto } from "../types/book.types";
import { publicService } from "../lib/services/public.service";
import { favouriteService } from "../lib/services/favourite.service";
import { useAuth } from "../contexts/AuthContext";
import { useToast } from "../components/ui/Toast";
import { Button } from "../components/ui/Button";
import { Spinner } from "../components/ui/Spinner";
import { Badge } from "../components/ui/Badge";
import { cn } from "../lib/utils";

export default function BookDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user } = useAuth();
  const { addToast } = useToast();

  const [book, setBook] = useState<BookDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isFavourite, setIsFavourite] = useState(false);

  useEffect(() => {
    const fetchBook = async () => {
      if (!id) return;
      setIsLoading(true);
      try {
        const data = await publicService.getBookById(id);
        setBook(data);
        
        // Initial favourite check
        if (!user) {
          setIsFavourite(favouriteService.isLocalFavourite(id));
        } else {
          // Assume local storage is synced for now
          setIsFavourite(favouriteService.isLocalFavourite(id));
        }
      } catch (error) {
        addToast("error", "Failed to load book details");
      } finally {
        setIsLoading(false);
      }
    };

    fetchBook();
  }, [id, user, addToast]);

  const toggleFavourite = async () => {
    if (!book) return;
    
    if (!user) {
      const isFav = favouriteService.toggleLocalFavourite(book.id);
      setIsFavourite(isFav);
      addToast("success", isFav ? "Added to favourites" : "Removed from favourites");
    } else {
      try {
        if (isFavourite) {
          await favouriteService.removeFavourite(book.id);
          setIsFavourite(false);
          favouriteService.toggleLocalFavourite(book.id); // keep local in sync
        } else {
          await favouriteService.addFavourite(book.id);
          setIsFavourite(true);
          favouriteService.toggleLocalFavourite(book.id);
        }
      } catch (error) {
        addToast("error", "Failed to update favourite");
      }
    }
  };

  const handleAction = (action: "borrow" | "reserve") => {
    if (!book) return;
    
    if (!user) {
      addToast("warning", `Please login to ${action} this book.`);
      navigate("/login"); // or show a modal, but redirecting is simpler and standard
      return;
    }

    if (action === "borrow") {
      navigate(`/borrows?action=new&bookId=${book.id}`);
    } else {
      navigate(`/reservations?action=new&bookId=${book.id}`);
    }
  };

  if (isLoading) {
    return (
      <div className="flex min-h-[500px] items-center justify-center">
        <Spinner size="lg" />
      </div>
    );
  }

  if (!book) {
    return (
      <div className="container mx-auto py-12 px-4 text-center">
        <h2 className="text-2xl font-bold mb-4">Book Not Found</h2>
        <Button onClick={() => navigate("/catalog")}>Back to Catalog</Button>
      </div>
    );
  }

  return (
    <div className="container mx-auto py-8 px-4 max-w-5xl">
      <Button
        variant="ghost"
        className="mb-6 -ml-4"
        onClick={() => navigate(-1)}
      >
        <ArrowLeft className="mr-2 h-4 w-4" />
        Back
      </Button>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-8 md:gap-12">
        {/* Cover Image */}
        <div className="md:col-span-1">
          <div className="aspect-[3/4] w-full overflow-hidden rounded-xl border bg-muted shadow-md">
            {book.coverImageUrl ? (
              <img
                src={book.coverImageUrl}
                alt={book.title}
                className="h-full w-full object-cover"
              />
            ) : (
              <div className="flex h-full w-full items-center justify-center text-6xl">
                📚
              </div>
            )}
          </div>
        </div>

        {/* Book Details */}
        <div className="md:col-span-2 flex flex-col">
          <div className="mb-4">
            <Badge className="mb-3">{book.categoryName}</Badge>
            <h1 className="text-3xl md:text-4xl font-bold tracking-tight mb-2">
              {book.title}
            </h1>
            <p className="text-xl text-muted-foreground flex items-center">
              <User className="mr-2 h-5 w-5" />
              {book.authorName}
            </p>
          </div>

          <div className="flex items-center space-x-6 text-sm text-muted-foreground mb-8 pb-8 border-b">
            <div>
              <span className="font-medium text-foreground">ISBN:</span> {book.isbn}
            </div>
            <div>
              <span className="font-medium text-foreground">Published:</span> {book.publicationYear}
            </div>
          </div>

          {/* Description */}
          <div className="mb-8 flex-1">
            <h3 className="font-semibold text-lg mb-2">Description</h3>
            <p className="text-muted-foreground leading-relaxed">
              {book.description || "No description available for this book."}
            </p>
          </div>

          {/* Actions */}
          <div className="bg-card border rounded-xl p-6 shadow-sm">
            <div className="flex flex-col sm:flex-row justify-between sm:items-center gap-4 mb-6">
              <div>
                <h4 className="font-medium flex items-center mb-1">
                  <Building className="mr-2 h-4 w-4 text-muted-foreground" />
                  Availability Status
                </h4>
                <div className="text-sm">
                  {book.availableCopies > 0 ? (
                    <span className="text-green-600 dark:text-green-400 font-medium">
                      {book.availableCopies} available out of {book.totalCopies} copies
                    </span>
                  ) : (
                    <span className="text-red-500 font-medium">
                      Currently unavailable (all {book.totalCopies} copies borrowed)
                    </span>
                  )}
                </div>
              </div>
              
              <div className="flex items-center gap-3">
                <Button
                  variant="outline"
                  size="icon"
                  className={cn("shrink-0", isFavourite && "text-red-500 border-red-200 dark:border-red-900/50 bg-red-50 dark:bg-red-900/20")}
                  onClick={toggleFavourite}
                  title="Add to Favourites"
                >
                  <Heart className={cn("h-5 w-5", isFavourite && "fill-current")} />
                </Button>
                
                {book.availableCopies > 0 ? (
                  <Button size="lg" className="w-full sm:w-auto" onClick={() => handleAction("borrow")}>
                    <BookOpen className="mr-2 h-4 w-4" />
                    Borrow Now
                  </Button>
                ) : (
                  <Button size="lg" className="w-full sm:w-auto" onClick={() => handleAction("reserve")}>
                    <Check className="mr-2 h-4 w-4" />
                    Reserve
                  </Button>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
