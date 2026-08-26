          $solutions = Get-ChildItem -Path "./../src/Avalonia.Samples" -Filter "*.sln*" -Recurse | Select-Object -ExpandProperty FullName
          foreach ($solution in $solutions) {
            Write-Host "=== Testing: $solution ==="
            dotnet test $solution --no-build --verbosity:detailed
          }
