#!/bin/bash

# Create a migration script to update database references
echo "Creating migration script..."

# Step 1: Replace all PhantomGGContext references with ApplicationDbContext
echo "Updating context references..."
find . -type f -name "*.cs" -exec sed -i 's/PhantomGGContext/ApplicationDbContext/g' {} \;

# Step 2: Update User references to ApplicationUser
echo "Updating user model references..."
find . -type f -name "*.cs" -exec sed -i 's/Models\.User/Models.ApplicationUser/g' {} \;

echo "Migration complete!"
