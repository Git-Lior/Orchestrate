import Typography from "@material-ui/core/Typography";
import React from "react";

interface Props {
  error: orch.Error;
  className?: string;
}

export function ErrorText({ className, error: { error, errors } }: Props) {
  if (error)
    return (
      <Typography variant="body1" color="error" className={className}>
        {error}
      </Typography>
    );

  if (!errors) return null;

  return (
    <div className={className}>
      {Object.entries(errors).map(([field, fieldErrors]) => (
        <Typography key={field} variant="body1" color="error">
          {field}: {fieldErrors[0]}
        </Typography>
      ))}
    </div>
  );
}
