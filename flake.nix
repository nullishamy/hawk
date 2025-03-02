{
  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs/nixpkgs-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  description = "Development shell flake";
  outputs = { self, nixpkgs, flake-utils }: flake-utils.lib.eachDefaultSystem (system: {
    devShell = let 
      pkgs = nixpkgs.legacyPackages.${system};
    in 
      pkgs.mkShell {
        packages = with pkgs; [
          dotnet-sdk
        ];
      };
  });
}
