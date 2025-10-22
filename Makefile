# Makefile for AvaloniaWithoutXAML projects

# Variables
DOTNET = dotnet
PROJECT ?= HelloWorldWindow
PROJECT_DIR = samples/$(PROJECT)
PROJECT_PROJ = $(PROJECT_DIR)/$(PROJECT).csproj

# Default target
.PHONY: all
all: build

# Project targets
.PHONY: build
build:
	@echo "Building $(PROJECT)..."
	$(DOTNET) build $(PROJECT_PROJ)

.PHONY: run
run: build
	@echo "Running $(PROJECT)..."
	$(DOTNET) run --project $(PROJECT_PROJ)

.PHONY: watch
watch:
	@echo "Running $(PROJECT) in watch mode..."
	$(DOTNET) watch run --project $(PROJECT_PROJ)

.PHONY: clean-project
clean-project:
	@echo "Cleaning $(PROJECT)..."
	$(DOTNET) clean $(PROJECT_PROJ)


# Utility targets
.PHONY: restore
restore:
	@echo "Restoring packages for $(PROJECT)..."
	$(DOTNET) restore $(PROJECT_PROJ)

.PHONY: clean
clean: clean-project
	@echo "Cleaning all projects..."

.PHONY: list-projects
list-projects:
	@echo "Available projects:"
	@ls -1 samples | grep -v README.md | sort

.PHONY: help
help:
	@echo "Available targets:"
	@echo "  build (PROJECT=name)     - Build specified project (default: HelloWorldWindow)"
	@echo "  run (PROJECT=name)      - Build and run specified project"
	@echo "  watch (PROJECT=name)    - Run specified project in watch mode"
	@echo "  clean-project (PROJECT=name) - Clean specified project"
	@echo "  restore (PROJECT=name)  - Restore packages for specified project"
	@echo "  list-projects           - List all available projects"
	@echo "  clean                   - Clean all projects"
	@echo "  help                    - Show this help message"
	@echo ""
	@echo "Examples:"
	@echo "  make                     # Build HelloWorldWindow"
	@echo "  make PROJECT=ButtonHandler build  # Build ButtonHandler project"
	@echo "  make PROJECT=Chex run    # Build and run Chex project"
	@echo "  make PROJECT=Harmonograph watch  # Run Harmonograph in watch mode"
	@echo "  make list-projects       # Show all available projects"