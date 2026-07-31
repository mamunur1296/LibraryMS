import { Link } from "react-router-dom";
import { Button } from "../components/ui/Button";

export function NotFoundPage() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center text-center px-4">
      <div className="space-y-4">
        <h1 className="text-9xl font-extrabold tracking-tighter text-primary/20">404</h1>
        <h2 className="text-3xl font-bold tracking-tight">Page not found</h2>
        <p className="text-muted-foreground max-w-[500px] mx-auto">
          Sorry, we couldn't find the page you're looking for. It might have been moved,
          deleted, or perhaps it never existed in the first place.
        </p>
        <div className="pt-6">
          <Link to="/">
            <Button size="lg">Go back home</Button>
          </Link>
        </div>
      </div>
    </div>
  );
}
